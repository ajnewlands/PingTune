using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingTune
{
    public class Tunable
    {
        Object cur;
        Object opt;
        Object def;

        RegistryValueKind type;

        public bool checkAndCommit( RegistryKey key, string subkey )
        {
            if (cur != key.GetValue(subkey, def))
                key.SetValue(subkey, cur, type);
            return true;
        }

        public RegistryValueKind getKind()
        {
            return type;
        }

        public Object getCurrent()
        {
            return cur;
        }

        public Object getDefault()
        {
            return def;
        }

        public bool isOptimized()
        {
            return (cur.Equals( opt ));
        }

        public bool setOptimized()
        {
            cur = opt;
            return true;
        }
        public bool setDefault()
        {
            cur = def;
            return true;
        }

        public Tunable( Object current, Object dflt, Object optimal, RegistryValueKind tp)
        {
            cur = current;
            def = dflt;
            opt = optimal;
            type = tp;
        }
    }
}
