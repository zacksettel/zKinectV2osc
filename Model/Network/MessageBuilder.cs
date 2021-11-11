using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.Kinect;
using Rug.Osc;

namespace zKinectV2OSC.Model.Network
{
    public class MessageBuilder
    {
        public OscMessage BuildJointMessage(Body body, Joint joint, Vector3 position, int indexId)
        {
            var address = String.Format("/kinect2/{0}/joint/{1}", indexId, joint.JointType);
            return new OscMessage(address, position.X, position.Y, position.Z);
        }

        public OscMessage BuildBodyStatusMessage(int indexId, bool state)
        {
            var address = String.Format("/kinect2/{0}/tracking", indexId);
            return new OscMessage(address, state);
        }
        public OscMessage Build6DjointMessage(Body body, Joint joint, System.Numerics.Vector3 p, System.Numerics.Vector4 q, int indexId)
        {
            var address = String.Format("/kinect2/{0}/6Djoint/{1}", indexId, joint.JointType);

            //System.Diagnostics.Debug.WriteLine(address);
            return new OscMessage(address, p.X, p.Y, p.Z, q.X, q.Y, q.Z, q.W);
        }


        public OscMessage BuildHandMessage(Body body, string key, HandState state,  int indexId)
        {
            var address = String.Format("/kinect2/{0}/hands/{1}", indexId, key);

            //System.Diagnostics.Debug.WriteLine(address);
            return new OscMessage(address, state.ToString() );
        }

        public OscMessage BuildLeanMessage(Body body, int indexId)
        {
            var address = String.Format("/kinect2/{0}/lean", indexId);

            //System.Diagnostics.Debug.WriteLine(address);
            return new OscMessage(address, body.Lean.X, body.Lean.Y);
        }

        public OscMessage BuildSkelFrameMessage(int indexId)
        {
            var address = String.Format("/kinect2/{0}/skelFrame", indexId);

            //System.Diagnostics.Debug.WriteLine(address);
            return new OscMessage(address);
        }

        public OscMessage BuildQuatMessage(System.Numerics.Vector4 q, System.Numerics.Vector3 p,  int indexId)
        {
            var address = String.Format("/kinect2/{0}/6dof", indexId);

            //System.Diagnostics.Debug.WriteLine(address);
            return new OscMessage(address, p.X, p.Y, p.Z, q.X, q.Y, q.Z, q.W);
        }
    }
}
