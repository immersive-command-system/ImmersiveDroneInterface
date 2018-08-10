using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class EnvironmentMsg : ROSBridgeMsg
        {
            public string id_;
            public float x_, y_, z_;
            public float x_rot_, y_rot_, z_rot_, w_rot_;

            public EnvironmentMsg(JSONNode msg)
            {

                id_ = msg["transforms"][0]["child_frame_id"];

                x_ = float.Parse(msg["transforms"][0]["transform"]["translation"]["x"]);
                y_ = float.Parse(msg["transforms"][0]["transform"]["translation"]["y"]);
                z_ = float.Parse(msg["transforms"][0]["transform"]["translation"]["z"]);

                x_rot_ = float.Parse(msg["transforms"][0]["transform"]["rotation"]["x"]);
                y_rot_ = float.Parse(msg["transforms"][0]["transform"]["rotation"]["y"]);
                z_rot_ = float.Parse(msg["transforms"][0]["transform"]["rotation"]["z"]);
                w_rot_ = float.Parse(msg["transforms"][0]["transform"]["rotation"]["w"]);
            }

            public EnvironmentMsg(float x, float y, float z)
            {
                x_ = x;
                y_ = y;
                z_ = z;
            }
        }
    }
}
