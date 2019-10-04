using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class TrajectoryBoundMsg : ROSBridgeMsg
        {
            public float _x, _y, _z;
            public int id;
            public string ns;
            public float _a, _r, _g, _b;

            public TrajectoryBoundMsg(JSONNode msg)
            {

                ns = msg["ns"];
                //x,y,z are scaled
                _x = float.Parse(msg["scale"]["x"]);
                _y = float.Parse(msg["scale"]["y"]);
                _z = float.Parse(msg["scale"]["z"]);
                //rgb color for visualization
                _a = float.Parse(msg["color"]["a"]);
                _r = float.Parse(msg["color"]["r"]);
                _g = float.Parse(msg["color"]["g"]);
                _b = float.Parse(msg["color"]["b"]);

                id = int.Parse(msg["id"]); ;
            }
        }
    }
}
