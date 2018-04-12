using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

namespace ROSBridgeLib
{
    namespace std_msgs
    {
        public class WaypointUpdateMsg : ROSBridgeMsg
        {
            public float _x, _y, _z;
            public int id;

            public WaypointUpdateMsg(JSONNode msg)
            {
                _x = float.Parse(msg["position"]["x"]);
                _y = float.Parse(msg["position"]["y"]);
                _z = float.Parse(msg["position"]["z"]);
                id = int.Parse(msg["id"]);
            }

            public WaypointUpdateMsg(float x, float y, float z)
            {
                _x = x;
                _y = y;
                _z = z;
            }
        }
    }
}
