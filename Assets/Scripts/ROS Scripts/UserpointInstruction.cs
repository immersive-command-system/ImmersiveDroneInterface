using SimpleJSON;
using System.Text;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class UserpointInstruction : ROSBridgeMsg
        {
            public string curr_id;
            public string prev_id;
            public double x;
            public double y;
            public double z;
            public string action;

            public UserpointInstruction(string curr_id_, string prev_id_, float x_, float y_, float z_, string action_)
            {

                curr_id = curr_id_;
                prev_id = prev_id_;

                // Translation into ROS space
                x = (double) x;
                y = (double) -z;
                z = (double) (y - 0.148f);

                action = action_;
            }

            public override string ToYAMLString()
            {
                return JsonUtility.ToJson(this);
            }


            // Not sure this is necessary - but may be useful for the ROSpoint messages

            //private const float table_height = 0.976f;
            //public UserpointInstruction(JSONNode msg)
            //{
            //    double x_ = double.Parse(msg["position"]["x"]);
            //    double y_ = double.Parse(msg["position"]["y"]);
            //    double z_ = double.Parse(msg["position"]["z"]);

            //    position = new double[3];
            //    position[0] = x_;
            //    position[1] = -z_;
            //    position[2] = (y_ - 0.148f) - table_height;
            //}
        }
    }
}
