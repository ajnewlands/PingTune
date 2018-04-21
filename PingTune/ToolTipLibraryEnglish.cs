using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingTune
{
    class ToolTipLibraryEnglish : IToolTipLibrary
    {
        Dictionary<string, string> _topics;

        public ToolTipLibraryEnglish()
        {
            _topics = new Dictionary<string, string>();
            _topics["toggleFlowControl"] = "Toggling adapter level flow control off may improve latency for both TCP and UDP by reducing processing time on the NIC's relatively slow processor.\n" +
                "This has limited downside insofar as TCP implements flow control of it's own making this superfluous from a reliability standpoint.\n" +
                "Similarly, UDP is by definition an unreliable transmission mechanism and so the programmer implementing UDP communications must implement a reliability mechanism in any event.\n";

            _topics["toggleMaxAckFreq"] = "By default, Windows will send ACKs for every other TCP segment, or 200ms after a TCP segment was received.\n" +
                "This can be disabled, and an ACK sent for every packet.\n" +
                "This can benefit TCP latency in some circumstances at the expense of bandwidth.";

            _topics["toggleMaxTxBuf"] = "Each buffer allows the NIC to track one or more packets in system memory.\n"
                + "Increasing this value may improve network performance (both TCP and UDP) at the expense of system memory utilization.";

            _topics["toggleMaxRxBuf"] = "Increasing this allows the NIC to utilize more system memory to hold received packets.\n"
                + "Increasing this value may improve network performance (both TCP and UDP) at the expense of system memory utilization";

            _topics["toggleNagle"] = "Nagle's algorithm aims to reduce the number of small packets sent onto the network.\n"
                + "It does so by holding packets briefly on the way out and attempting to combine them into bigger packets.\n"
                + "Disabling this behaviour can benefit TCP latency at the expense of network congestion/throughput.";

            _topics["toggleInterruptModeration"] = "When interrupt moderation is enabled, the NIC will delay sending interrupts to the CPU until multiple packets are received.\n"
                + "This reduces the number of interrupts the CPU must process, thereby limiting processor load, but may increase latency.\n"
                + "Removing this delay results in more interrupts (CPU load) but can reduce latency for both TCP and UDP.";

            _topics["toggleUdpOffload"] = "Many NICs support offloading the verification of the UDP header checksum to the NIC's processor.\n"
                + "This can reduce load on the CPU, however the NIC's processor is generally significantly slower than the CPU.\n"
                + "Where CPU resources are not at a premium, turning this off can reduce UDP latency.";

            _topics["toggleTcpOffload"] = "Many NICs support offloading the verification of the TCP header checksum to the NIC's processor.\n"
                + "This can reduce load on the CPU, however the NIC's processor is generally significantly slower than the CPU.\n"
                 + "Where CPU resources are not at a premium, turning this off can reduce TCP latency.";

            _topics["toggleRssDisable"] = "Receive Side Scaling allows network processing to be shared across multiple processors.\n"
                + "This should generally be a good thing. However, in earlier versions of Windows (e.g. server 2003), the implementation was very buggy.\n"
                + "In modern versions of Windows this should be safe to enable. Note, however, that it will be implicitly disabled when checksum offloading is disabled.";

            _topics["toggleHeadDataSplit"] = "Header data splitting splits the ethernet frame into separate buffers for header and payload.\n"
                + "This is intended to reduce the overhead for memory access in the network driver stack and should be a good thing.";

            _topics["toggleAckDelay"] = "This setting varies the interval between TCP segments that mark the end of a TCP stream.\n"
                + "The default is 2ms but it can be removed altogether. This will generate more ACKs (reducing efficiency/throughput) but may benefit TCP latency.";

            _topics["toggleLargeSendOffload"] = "Large send offload is designed to reduce CPU overhead.\n"
                + "When enabled, the system will send a large(multi-packet) buffer to the NIC. The NIC is then responsible for splitting this into multiple packets.\n"
                + "However, this can result in increased packet fragmentation when the NIC assumes an MTU of 1500.\n"
                + "This optimization has a bad reputation across the board - it is generally beneficial to disable it.";

            _topics["toggleThrottling"] = "By default Windows restricts processing of network packets to roughly 10 per millisecond when multimedia applications are running.\n"
                + "This can be detrimental to latency for e.g. online games. Turning this off can improve latency for both TCP and UDP at the expense of CPU overhead.";

            _topics["toggleMaxForePriority"] = "By default, Windows will give background processes priority access to 20% of the CPU (more on server editions).\n"
                + "This can be turned off to maximize the performance of the current foreground application, at the expense of background process performance.";

            _topics["MtuOptButton"] = "By default, Windows will assume an MTU of 1500 for ethernet. If this does not match your upstream ISP then packet fragmentation will occur.\n"
                + "This is a bad thing - by setting the correct MTU here (and ideally on your router as well) you will prevent this from happening.\n"
                + "This will directly benefit latency.";

        }

        public string getToolTip( string topic)
        {
            try
            {
                return _topics[topic];
            }
            catch (KeyNotFoundException)
            {
                return "Missing tooltip for " + topic;
            }
        }
    }
}
