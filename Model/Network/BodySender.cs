using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Numerics;
using Microsoft.Kinect;
using Rug.Osc;

namespace zKinectV2OSC.Model.Network
{
    public class BodySender
    {

        private MessageBuilder messageBuilder;
        private List<OscSender> oscSenders;
        private List<IPAddress> ipAddresses;
        private OscMessage message;
        private string port;
        private string status;
        private static int maxUserCount = 6;
        private Enum[] LHstate = new Enum[6];
        private Enum[] RHstate = new Enum[6];

        public bool jointTX = true;
        public float depthClip = 4.5f;


        public BodySender(string delimitedIpAddresses, string port)
        {
            this.status = "";
            this.ipAddresses = this.Parse(delimitedIpAddresses);
            this.oscSenders = new List<OscSender>();
            this.port = port;
            this.messageBuilder = new MessageBuilder();
            this.TryConnect();

            for (int i = 0; i < 6; i++)
            {
               RHstate[i] = LHstate[i] = HandState.NotTracked;
            }
        }



        private void TryConnect()
        {
            foreach (var ipAddress in this.ipAddresses)
            {
                try
                {
                    var oscSender = new OscSender(ipAddress, int.Parse(this.port));
                    oscSender.Connect();
                    this.oscSenders.Add(oscSender);
                    this.status += "OSC connection established on\nIP: " + ipAddress + "\nPort: " + port + "\n";
                }
                catch (Exception e)
                {
                    this.status += "Unable to make OSC connection on\nIP: " + ipAddress + "\nPort: " + port + "\n";
                    Console.WriteLine("Exception on OSC connection...");
                    Console.WriteLine(e.StackTrace);
                }
            }

        }

        public void Send(Body[] bodies)
        {
            int index = 0;
            foreach (Body body in bodies)
            {
                if (body.IsTracked)
                {
                    this.Send(body, index);
                }
                index++;
            }
        }

        public string GetStatusText()
        {
            return this.status;
        }

        private void Send(Body body, int bodiesIndex)
        {
            bool newSkelData = false;

            if (body.Joints[JointType.SpineMid].Position.Z > depthClip) return;

            // use SpineMid as body origin
            System.Numerics.Vector3 bodyPosition = new System.Numerics.Vector3(
                body.Joints[JointType.SpineMid].Position.X,
                body.Joints[JointType.SpineMid].Position.Y,
                body.Joints[JointType.SpineMid].Position.Z);

            // Could add an out-of-bounds message here when body parts are out if kinect's view 
            //if (body.ClippedEdges.HasFlag(FrameEdges.Bottom))

            if (jointTX)
            {
                System.Numerics.Vector3 relPos = new System.Numerics.Vector3();
                foreach (var joint in body.Joints)
                {
                    if (joint.Value.TrackingState.Equals(TrackingState.Tracked))
                    {
                        relPos.X = joint.Value.Position.X;
                        relPos.Y = joint.Value.Position.Y;
                        relPos.Z = joint.Value.Position.Z;  

                        relPos -= bodyPosition;  // make joint relative to body
                        relPos.Z = -relPos.Z;  // negate this puppy to unMirror

                        message = messageBuilder.BuildJointMessage(body, joint.Value, relPos, bodiesIndex);
                        this.Broadcast(message);
                        newSkelData = true;
                    }
                }
            }

            if (body.HandLeftConfidence.Equals(TrackingConfidence.High) ||
                body.HandLeftState.Equals(HandState.NotTracked) )
            {
                if ( LHstate[bodiesIndex].Equals(body.HandLeftState) == false ) // ignore redundent values
                {
                    LHstate[bodiesIndex] = body.HandLeftState; 
                    message = messageBuilder.BuildHandMessage(body, "Left", body.HandLeftState, bodiesIndex);
                    this.Broadcast(message);
                    //newSkelData = true;
                }
           }

            if (body.HandRightConfidence.Equals(TrackingConfidence.High) ||
                body.HandRightState.Equals(HandState.NotTracked))
            {
                if (RHstate[bodiesIndex].Equals(body.HandRightState) == false) // ignore redundent values
                {
                    RHstate[bodiesIndex] = body.HandRightState;
                    message = messageBuilder.BuildHandMessage(body, "Right", body.HandRightState, bodiesIndex);
                    this.Broadcast(message);
                    //newSkelData = true;
                }
            }


            if (newSkelData)
            {
                message = messageBuilder.BuildSkelFrameMessage(bodiesIndex);
                this.Broadcast(message);
            }

            if (body.LeanTrackingState.Equals(TrackingState.Tracked))
            {
                message = messageBuilder.BuildLeanMessage(body, bodiesIndex);
                this.Broadcast(message);
            }
            // using the code below, we could calculate our own rotation but the built-in one looks pretty good
            // if we have a stable pose calculate global rotation
            /*            if (body.Joints[JointType.SpineBase].TrackingState.Equals(TrackingState.Tracked)
                           && body.Joints[JointType.ShoulderLeft].TrackingState.Equals(TrackingState.Tracked)
                           && body.Joints[JointType.ShoulderRight].TrackingState.Equals(TrackingState.Tracked))
                           */

            // use the built-in rotation
            if ( body.Joints[JointType.SpineMid].TrackingState.Equals(TrackingState.Tracked) )
            {
                Vector3 pos = new Vector3(body.Joints[JointType.SpineMid].Position.X, body.Joints[JointType.SpineMid].Position.Y, body.Joints[JointType.SpineMid].Position.Z);

                Quaternion jQuat = new Quaternion(body.JointOrientations[JointType.SpineMid].Orientation.X,
                                             body.JointOrientations[JointType.SpineMid].Orientation.Y,
                                             body.JointOrientations[JointType.SpineMid].Orientation.Z,
                                             body.JointOrientations[JointType.SpineMid].Orientation.W);
                Quaternion invXqut = Quaternion.CreateFromYawPitchRoll( (float)Math.PI,  0,  0 ); // create inverse X axix quat
                Quaternion jQuatInv = Quaternion.Multiply(jQuat, invXqut);   // flip it around so its not mirrored

                System.Numerics.Vector4 outRot = new System.Numerics.Vector4(-1, -1, 1, 1);  // these are the needed inverseScalers so it works well with Unity

                outRot *= new System.Numerics.Vector4(jQuatInv.X, jQuatInv.Y, jQuatInv.Z, jQuatInv.W);  // oriented for Unity3D


                message = messageBuilder.BuildQuatMessage(outRot, pos,  bodiesIndex);
                this.Broadcast(message);

            }


        }

        private void Broadcast(OscMessage message)
        {
            foreach (var oscSender in this.oscSenders)
            {
                oscSender.Send(message);
            }
        }

        private void getBodyQuat (Body bod)
        {     
            Vector3 u,  v;
            // enough points to calculate a rotation?
            if (bod.Joints[JointType.SpineBase].TrackingState.Equals(TrackingState.NotTracked)
                || bod.Joints[JointType.ShoulderLeft].TrackingState.Equals(TrackingState.NotTracked)
                || bod.Joints[JointType.ShoulderRight].TrackingState.Equals(TrackingState.NotTracked))
                return;
            // else calculate and send rotation

            u = new Vector3();
            v = new Vector3();



            float m = (float)Math.Sqrt(2 + 2 * System.Numerics.Vector3.Dot(u, v));
            Vector3 w = (1f / m) * System.Numerics.Vector3.Cross(u, v);
            //return new System.Numerics.Vector4(0.5f * m, w.X, w.Y, w.Z);
            return;
        }

        private List<IPAddress> Parse(string delimitedIpAddresses)
        {
            try
            {
                var ipAddressStrings = delimitedIpAddresses.Split(',');
                var ipAddresses = new List<IPAddress>();
                foreach (var ipAddressString in ipAddressStrings)
                {
                    ipAddresses.Add(IPAddress.Parse(ipAddressString));
                }
                return ipAddresses;
            }
            catch (Exception e)
            {
                status += "Unable to parse IP address string: '" + delimitedIpAddresses + "'";
                Console.WriteLine("Exception parsing IP address string...");
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
    }
}
