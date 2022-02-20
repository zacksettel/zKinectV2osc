KinectV2OSC
===========
Broadcasts [KinectV2](http://www.microsoft.com/en-us/kinectforwindows/purchase/) skeletal data over OSC. That's it.

![A Kinect V2 sensor](kinect.jpg)

Handy if you want to quickly get skeleton data off of Windows and onto Mac, or into some other Windows app.

Just run the app
----------------
If you just want to run the app without digging into the code, you can simply follow these instructions:
- Note that you need Windows 8.1, a USB3 port, and a new V2 Kinect sensor
- Download and install the [Kinect for Windows SDK 2.0](http://www.microsoft.com/en-us/kinectforwindows/develop/default.aspx)
- Download the release executable and run it. You should see a screen like this:

![Screenshot of KinectV2OSC in action](screenshot.png)

- By default, the executable broadcasts to port `12345` on IP address `127.0.0.1`
- To broadcast elsewhere, create a file named `ip.txt` in the same folder as the executable. Give the file a comma-separated list of IP asddresses to broadcast to, i.e.

```sh
127.0.0.1,192.168.1.100,192.168.1.112
```

- Re-launch the executable

Compile from source
-------------------
If you want to be able to change the port number, or edit the code, you will need to compile from source. Follow these instructions:
- Note that you need Windows 8.1, a USB3 port, and a new V2 Kinect sensor
- Download and install the [Kinect for Windows SDK 2.0](http://www.microsoft.com/en-us/kinectforwindows/develop/default.aspx)
- Install Visual Studio (I am using [Visual Studio Express 2013 for Windows Desktop](http://www.visualstudio.com/en-us/products/visual-studio-express-vs.aspx) - scroll down to find the download link)
- MAKE SURE you are using Visual Studio Express for **Windows Desktop** - you have to scroll down to find that link!!
- Clone this repo, and open KinectV2OSC.sln in Visual Studio. Hit the green 'Start' button. You should see a screen like this:

![Screenshot of KinectV2OSC in action](screenshot.png)

- To send to another destination, change the IP and port number here, and then re-launch:

![How to configure IP and port number](config.png)

You can enter a single IP Address or multiple, separated by commas.

What OSC messages get sent?
---------------------------
The message set has been extended from the original source code. Joints are in local coordinates, relative to the SpineMid joint

A new message to mark the beginnning of each frame of skeletel messages.

```sh
Address: /kinect2/{bodyId: 0 - 5 }/skelFrame
```

The original message protocol for joints has been modidied to the following protocal:

```sh
Address: /kinect2/{bodyId: 0 - 5 }/joint/{jointId}
Values: - float:  positionX
        - float:  positionY
        - float:  positionZ
```
message protocol for hands:

```sh
Address: /kinect2/{bodyId: 0 - 5 }/hands/Left or  ..../Right 
Value - symbol:  Open, CLosed, or NotTracked
```
new message for the user position and orientation (relative to SpineMid)

```sh
Address: /kinect2/{bodyId: 0 - 5 }/6dof
Values: - float:  positionX
        - float:  positionY
        - float:  positionZ
        - float:  quaternianX
        - float:  quaternianY
        - float:  quaternianZ
        - float:  quaternianW
        
 Address: /kinect2/{bodyId: 0 - 5 }/lean        
        - float:  leanFrontRear
        - float: leanLeftRight
```

Command Line Argument Flags offer the following options:
---------------------------

            -p , --port, DefaultValue = 12345


            -i, --ip, DefaultValue = 127.0.0.1 (localhost)   note that UDP broadast is available 


            -r, --oscUpdateRateMs, DefaultValue = 50 ms


            -d,  --depthClip", DefaultValue = 4.5 meters


            -j, --jointTX, DefaultValue = 1 (enable/disable osc joint messages)

Project dependencies
--------------------
- [Rug OSC](https://www.nuget.org/packages/Rug.Osc/) to format and send messages
