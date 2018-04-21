using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PingTune
{
    class GlobalSystemState
    {
        public Tunable netThrottling;
        public Tunable backgroundReservedCpuPct;

        public GlobalSystemState()
        {
            RegistryKey hklm64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey reg = hklm64.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile");

            netThrottling = new Tunable(reg.GetValue("NetworkThrottlingIndex", 10), 10, 70, RegistryValueKind.DWord);
            backgroundReservedCpuPct = new Tunable(reg.GetValue("SystemResponsiveness", 20), 20, 0, RegistryValueKind.DWord);

            reg.Close();
            hklm64.Close();
        }

        public bool Commit()
        {
            RegistryKey hklm64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey reg = hklm64.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile",true);

            // Update the network throttling value in the registry.
            netThrottling.checkAndCommit(reg, "NetworkThrottlingIndex");

            // Update the System reponsiveness (reserved resources for background programs) in the registry.
            backgroundReservedCpuPct.checkAndCommit(reg, "SystemResponsiveness");

            reg.Close();
            hklm64.Close();
            return true;
        }
    }
}
