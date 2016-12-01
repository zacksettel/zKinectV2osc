zKinectV2osc
built on the project:

https://github.com/microcosm/KinectV2-OSC


changes to that code incude

joints are in local coordinates, relative to the SpineMid joint

modified message protocol for joints:

```sh
Address: /kinect/{bodyId: 0 - 5 }/joint/{jointId}
Values: - float:  positionX
        - float:  positionY
        - float:  positionZ
```
message protocol for hands:

```sh
Address: /kinect/{bodyId: 0 - 5 }/hands/Left or  ..../Right 
Value - symbol:  Open, CLosed, or NotTracked
```
new message for the user position and orientation (relative to SpineMid)

```sh
Address: /kinect/{bodyId: 0 - 5 }/6dof
Values: - float:  positionX
        - float:  positionY
        - float:  positionZ
        - float:  quaternianX
        - float:  quaternianY
        - float:  quaternianZ
        - float:  quaternianW
        
 Address: /kinect/{bodyId: 0 - 5 }/lean        
        - float:  leanFrontRear
        - float: leanLeftRight
```
There are also a some command line argument flags

            -p , --port, DefaultValue = 12345


            -i, --ip, DefaultValue = 127.0.0.1 (localhost)


            -r, --oscUpdateRateMs, DefaultValue = 50 ms


            -d,  --depthClip", DefaultValue = 4.5 meters


            -j, --jointTX, DefaultValue = 1 (enable/disable osc joint messages)
