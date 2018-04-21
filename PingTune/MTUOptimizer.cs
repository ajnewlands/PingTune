using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;


namespace PingTune
{
    public class MTUOptimizer
    {
        private MTUOptimizer()
        {

        }

        public static int getOptimalMTU()
        {
            var p = new Ping();

            var o = new PingOptions();
            o.DontFragment = true;


            for (int i = 1500; i > 0; i--)
            {
                byte[] buf = new byte[i];
                var r = p.Send("www.google.com", 2000, buf, o);
                if (r.Status == IPStatus.Success)
                {
                    return (i + 28); // Return the optimal MTU for this network.
                }
                else if (r.Status == IPStatus.PacketTooBig)
                {
                    // Fragmentation happened -> try something smaller.
                }
                else
                {
                    // Something unexpected has broken down - return default.
                    break;
                }
                
            }
            return 1500; // The limit for Ethernet.
        }
    }
}
