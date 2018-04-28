# What settings does PingTune expose?

## System wide settings

These settings are not specific to any particular network adapter but rather to Windows itself.

### Windows built in network throttling

Windows has a built in network throttling mechanism that limits processing of network packets to roughly 10 per millisecond whilst any designated multimedia application is running. This is intended to ensure that the CPU is not monopolized by processing network data at the expense of say video playback, resulting in stuttering. However, today's CPUs tend to have cores to spare whilst this throttling is definitely going to impact both latency and throughput when it kicks in. The 'Network Throttling' slider allows easy adjustment of this setting in increments of 10 packets per millisecond.

### Foreground application priority

Windows will by default ensure that background applications get at least 20% of the CPU time (more if it happens to be a designated server version of Windows). That's not so great when all that really matters is the single application in the foreground. The 'foreground application priority' slider offers the ability to adjust the percentage guaranteed to foreground applications to be anything from 0% through to 100% at will.

### (enable/disable) sharing windows updates with other PCs on the internet

Windows 10 introduced peer to peer sharing of windows updates with other PCs using a protocol similar to bittorrent.
Chances are if you are specifically trying to optimize for network latency, you absolutely don't want this to be happening - so here is the ability to easily toggle it on or off.

This won't disable sharing with other PCs on the same LAN; chances are this is something you want to leave on because it is very likely that your LAN connection is faster than your internet connection. In other words, it's less disruptive for your PC to share files with another PC on the LAN than it is for your PC to share an internet connection with another PC that is downloading large updates!

### (enable/disable) Xbox video recording

Recently, microsofy introduced functionality to record videos (primarily of gameplay) in the background of Windows 10. Many people have noticed increased latency, stutterings and slowdowns since this feature was enabled. Here is the option to turn it off permanently, which is highly recommended for the 99.9% of people who will never use it anyway!

## Network interface specific settings

These settings are specific to a single instance of a network adapter (i.e. ethernet card) and must be applied to the active interface to be effective. PingTune tries to default to the active interface where multiple are available, but in the event it does not, select the right one from the dropdown menu.

### (enable/disable) ACK frequency limits

By default, Windows will send an ACK at the end of every second TCP segment, or else 200ms after the last segment was received.
For latency critical situations, you may change this so that Windows will instead send an ACK after every segment, potentially eliminating some stall conditions.

### (enabled/disable) Network adapter flow control

Some network adapters implement their own flow control, on top of that built into the protocol level. This can improve bandwidth utilization but is often unnecessary extra overhead and may interfere with application level flow control. Disabling it can be benficial in latency critical scenarios.

### (enabled/disable) Delayed ACKs

By default Windows will consider a 2ms delay between TCP segments to mark the end of a stream. This can be reduced, resulting in more ACKs (bandwidth utilization) but potentially improved latency for TCP applications.

### (enable/disable) UDP header checksum offloading

Many network cards can compute header checksums themselves rather than rely on the operating system/CPU to do so. However, most network cards have comparatively slow processors whilst most modern machines are well endowed with CPU cores. Using the fast CPU instead of the slow network card is frequently beneficial for UDP latency.

### (enable/disable) TCP header checksum offloading

Many network cards can compute header checksums themselves rather than rely on the operating system/CPU to do so. However, most network cards have comparatively slow processors whilst most modern machines are well endowed with CPU cores. Using the fast CPU instead of the slow network card is frequently beneficial for TCP latency.

### (enable/disable) Allocate maximum allowable transmit buffers

Each TX buffer allows the NIC to track one or more packets in system memory. Increasing this can benefit network performance both in terms of latency and throughput, albeit at the expense of a small amount of system RAM. Unless system RAM is at a premium, there's no harm in turning this on.

### (enable/disable) Allocate maximum allowable receive buffers

RX buffers are used to hold received packets in system memory. Again, increasing this value can benefit network performance at the expense of a small amount of system RAM. Unless system RAM is at a premium, there's no harm in turning this on.

### (enable/disable) Nagle's algorithm

Nagle's algorithm aims to reduce the number of small packets sent onto the network in order to ease congestion. It delays the sending of small packets, with the aim being to aggregate them into single larger packets. This is beneficial for throughput but not for latency; if TCP latency is critical, turning it off will often help significantly.

### (enable/disable) Network adapter interrupt moderation

Rather than generate an interrupt as soon as a packet is received, some network adapters will wait, the aim being to generate a single interrupt for multiple packets. This reduces the number of interrupts the CPU must deal with, cutting overhead, but is obviously sub-optimal where latency is the primary concern.

### (enable/disable) Receive side scaling

Receive side scaling allows network processing to be split across multiple processors. It was known to be buggy in earlier versions of Windows (e.g. server 2003) and turning it off was typically recommended as a first step in debugging network issues. Regardless, if checksum offloading is disabled, this will not function. Typically I turn it off explicitly in any event.

### (enable/disable) Header buffer splitting

Windows now has the capability to split the headers and data payloads into separate buffers to process them separately. This should be beneficial although it is difficult to measure. Assuming you are already maximizing the available network buffers this seems like a reasonable way to put them to use!

### (enable/disable) Large send offloading

Large send offload was conceived as a way to cut CPU load. In short, rather than the CPU being responsible for sending many appropriately sized packets to the network adapter, this feature enables the CPU to send one big buffer full of data to the adapter, which is then responsible for carving this up into a number of appropriately sized packets. However, frequently the adapter will assume an MTU of 1500 which can result in excessive packet fragmentation when sending these packets beyond the LAN. It's better to turn it off, ensure the system MTU is set properly (see below) and let the operating system handle sending appropriate sized packets that won't fragment. (Fragmentation can result in latency spikes).

### Optimize the Maximum Transmission Unit (MTU)

The MTU is simply the largest packet that can be sent out over your ISPs connection to the internet. The default and maximum value is 1500, but typically the correct value (which depends upon your ISP) will be a little smaller. Figuring out the real value entails sending a series of packets, starting at 1500 bytes and gradually shrinking until you hit the largest size that doesn't result in fragmentation. PingTune automatically figures out the correct size for your network (if you are connected to the internet at the time you run it) and will set this for you at a touch of the 'optimize MTU' button.
