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
        - string: trackingState (Tracked, NotTracked or Inferred)
        - string: trackingState (Tracked, NotTracked or Inferred)
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
```
There are also a some command line argument flags

            -p , --port, DefaultValue = 12345


            -i, --ip, DefaultValue = 127.0.0.1 (localhost)


            -r, --oscUpdateRateMs, DefaultValue = 50 ms


            -d,  --depthClip", DefaultValue = 4.5 meters


            -j, --jointTX, DefaultValue = 1 (enable/disable osc joint messages)
