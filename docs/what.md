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

### (enabled/disable) Network adapter flow control

### (enabled/disable) Delayed ACKs

### (enable/disable) UDP header checksum offloading

### (enable/disable) TCP header checksum offloading

### (enable/disable) Allocate maximum allowable transmit buffers

### (enable/disable) Allocate maximum allowable receive buffers

### (enable/disable) Nagle's algorithm

### (enable/disable) Network adapter interrupt moderation

### (enable/disable) Receive side scaling

### (enable/disable) Header buffer splitting

### (enable/disable) Large send offloading
