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
            public float _x, _y, _z;
            public float _x_rot, _y_rot, _z_rot, _w_rot;

            public EnvironmentMsg(JSONNode msg)
            {
                _x = float.Parse(msg["pose"]["position"]["x"]);
                _y = float.Parse(msg["pose"]["position"]["y"]);
                _z = float.Parse(msg["pose"]["position"]["z"]);
                _x_rot = float.Parse(msg["pose"]["orientation"]["x"]);
                _y_rot = float.Parse(msg["pose"]["orientation"]["y"]);
                _z_rot = float.Parse(msg["pose"]["orientation"]["z"]);
                _w_rot = float.Parse(msg["pose"]["orientation"]["z"]);
            }

            public EnvironmentMsg(float x, float y, float z)
            {
                _x = x;
                _y = y;
                _z = z;
            }
        }
    }
}
