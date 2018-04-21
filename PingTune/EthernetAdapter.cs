using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingTune
{
    public class EthernetAdapter
    {
        public string Id { get; }
        public string Description { get; set; }
        public bool Connected { get; set; }
        public long Speed { get; set; }
        public string Address { get; set; }

        public string _ndis_key { get; set; } // The key windows uses within the registry Control structure to refer to this instance. Needed to write changes later.

        public Tunable FlowControl;
        public Tunable InterruptModeration;
        public Tunable RxBuffers;
        public Tunable TxBuffers;
        public Tunable TcpChecksumOffload;
        public Tunable UdpChecksumOffload;
        public Tunable Nagling;
        public Tunable TcpAckFrequency;
        public Tunable TcpDelayedAckTicks;
        public Tunable LargeSendOffload;
        public Tunable RSS;
        public Tunable HeaderDataSplit;


        public int MTU { get; set; } = 1500; // 1500 default according to ethernet standard.

        public EthernetAdapter( string id )
        {
            Id = id;
        }
        
        public bool Commit()
        {
            RegistryKey ndis_reg = Registry.LocalMachine.OpenSubKey("SYSTEM\\ControlSet001\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}" + "\\" + _ndis_key, true);

            FlowControl.checkAndCommit(ndis_reg, "*FlowControl");
            InterruptModeration.checkAndCommit(ndis_reg, "*InterruptModeration");
            RxBuffers.checkAndCommit(ndis_reg, "*ReceiveBuffers");
            TxBuffers.checkAndCommit(ndis_reg, "*TransmitBuffers");
            LargeSendOffload.checkAndCommit(ndis_reg, "*LsoV2IPv4");
            TcpChecksumOffload.checkAndCommit(ndis_reg, "*TCPChecksumOffloadIPv4");
            UdpChecksumOffload.checkAndCommit(ndis_reg, "*UDPChecksumOffloadIPv4");
            RSS.checkAndCommit(ndis_reg, "*RSS");
            HeaderDataSplit.checkAndCommit(ndis_reg, "*HeaderDataSplit");

            ndis_reg.Close();

            RegistryKey reg = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces" + "\\" + Id, true);
            TcpDelayedAckTicks.checkAndCommit(reg, "TcpDelAckTicks");
            Nagling.checkAndCommit(reg, "TCPNoDelay");
            TcpAckFrequency.checkAndCommit(reg, "TcpAckFrequency");

            // MTU behaves a little differently and is stored differently (for now anyway).
            if (MTU != int.Parse(reg.GetValue("MTU", "1500").ToString()))
                reg.SetValue("MTU", MTU.ToString(), RegistryValueKind.DWord);

            reg.Close();

            return true;
        }
    }
}
