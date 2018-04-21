using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Security;

namespace PingTune
{
    public class NetState
    {
        public static Dictionary<String, EthernetAdapter> _ethernetAdapters = new Dictionary<String, EthernetAdapter>();

        public static Dictionary<String,EthernetAdapter> getAdapterState()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces() )
            {
                IPInterfaceProperties p = adapter.GetIPProperties();
                var a = new EthernetAdapter(adapter.Id);
                a.Description = adapter.Description;
                a.Speed = adapter.Speed;
                

                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    a.Connected = true;                  
                }

                NetworkInterfaceType t = adapter.NetworkInterfaceType;

                foreach ( UnicastIPAddressInformation ip in p.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        // Crassly assume the first IPv4 address is primary!
                        a.Address =  ip.Address.ToString();
                        break;
                    }
                }
                if (t == NetworkInterfaceType.Ethernet)
                { // Keep a list of the interesting adapter IDs for later.
                    a.MTU = adapter.GetIPProperties().GetIPv4Properties().Mtu;
                    _ethernetAdapters.Add( adapter.Id, a );
                }
            }
            Console.WriteLine("Found {0} ethernet devices", _ethernetAdapters.Count());

            getRegNdisValues();

            return _ethernetAdapters;
        }



        private static void getRegNdisValues()
        {
            // THis is where network card configuration parameters live.
            // See https://blogs.technet.microsoft.com/networking/2009/01/08/configuring-advanced-network-card-settings-in-windows-server-2008-server-core/
            RegistryKey reg = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}", false);
            if (reg != null)
            {
                foreach (string keyName in reg.GetSubKeyNames())
                {
                    try
                    {
                        RegistryKey ndis_reg = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}" + "\\" + keyName, false);
                        string Id = ndis_reg.GetValue("NetCfgInstanceId", "").ToString();
                        if (Id != "")
                        { // Just in case we somehow open one of the non-NDIS keys despite the permissions on them.
                            if (_ethernetAdapters.ContainsKey(Id)) // Ensure we only examine the "interesting" ethernet adapters.
                            {
                                _ethernetAdapters[Id]._ndis_key = keyName;
                               
                                _ethernetAdapters[Id].FlowControl = new Tunable(ndis_reg.GetValue("*FlowControl", "3"), "3", "0", RegistryValueKind.String);
                                // Interrupt moderation current state and default value (well known values)
                                _ethernetAdapters[Id].InterruptModeration = new Tunable(ndis_reg.GetValue("*InterruptModeration", "1"), "1", "0", RegistryValueKind.String);
                                // Number of receive buffers - keys standard, values not. 
                                RegistryKey rx_buf_dflts = ndis_reg.OpenSubKey("Ndi\\Params\\*ReceiveBuffers", false);
                                _ethernetAdapters[Id].RxBuffers = new Tunable(ndis_reg.GetValue("*ReceiveBuffers", "256"),
                                    rx_buf_dflts.GetValue("default", "256"),
                                    rx_buf_dflts.GetValue("max", "256"),
                                    RegistryValueKind.String);
                                rx_buf_dflts.Close();

                                //Transmit buffers - keys are standard, values are not. 
                                RegistryKey tx_buf_dflts = ndis_reg.OpenSubKey("Ndi\\Params\\*TransmitBuffers", false);
                                _ethernetAdapters[Id].TxBuffers = new Tunable(ndis_reg.GetValue("*TransmitBuffers", "256"),
                                    tx_buf_dflts.GetValue("default", "256"),
                                    tx_buf_dflts.GetValue("max", "256"),
                                    RegistryValueKind.String);
                                tx_buf_dflts.Close();

                                // Large send offload - well known keys and known to cause trouble when enabled.
                                _ethernetAdapters[Id].LargeSendOffload = new Tunable(ndis_reg.GetValue("*LsoV2IPv4", "1"), "1", "0", RegistryValueKind.String);

                                // Offload settings for TCP and UDP (well known values)
                                _ethernetAdapters[Id].TcpChecksumOffload = new Tunable(ndis_reg.GetValue("*TCPChecksumOffloadIPv4", "3"), "3", "0", RegistryValueKind.String);
                                _ethernetAdapters[Id].UdpChecksumOffload = new Tunable(ndis_reg.GetValue("*UDPChecksumOffloadIPv4", "3"), "3", "0", RegistryValueKind.String);

                                // Receive side scaling (well known keys and values)
                                _ethernetAdapters[Id].RSS = new Tunable(ndis_reg.GetValue("*RSS", "1"), "1", "0", RegistryValueKind.String);

                                // Header data split - allows packets to be split into payload and header for concurrent processing.
                                // Assuming this is good until show otherwise, however implicitly disabled anyway if header checksum offload is disabled.
                                _ethernetAdapters[Id].HeaderDataSplit = new Tunable(ndis_reg.GetValue("*HeaderDataSplit", "0"), "0", "1", RegistryValueKind.String);
                            }
                            
                        }
                        ndis_reg.Close();
                    } catch (SecurityException)
                    {
                        // Some oddball keys in here are owned by TrustedInstaller and even Administrator will be told to sod off.
                        // luckily, we don't care about them.
                    }
                }
            }
            reg.Close();

            foreach (string Id in _ethernetAdapters.Keys)
            {
                reg = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces" + "\\" + Id, false);
                _ethernetAdapters[Id].TcpDelayedAckTicks = new Tunable(reg.GetValue("TcpDelAckTicks", "2"), 2, 0, RegistryValueKind.DWord);
                _ethernetAdapters[Id].Nagling = new Tunable(reg.GetValue("TCPNoDelay", "0"), 0, 1, RegistryValueKind.DWord);
                _ethernetAdapters[Id].TcpAckFrequency = new Tunable(reg.GetValue("TcpAckFrequency", "2"), 2, 1, RegistryValueKind.DWord);

                _ethernetAdapters[Id].MTU = int.Parse(reg.GetValue("MTU", "1500").ToString());
                reg.Close();
            }

        }
 
    }
}
