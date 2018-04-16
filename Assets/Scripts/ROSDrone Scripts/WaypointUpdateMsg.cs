using SimpleJSON;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class WaypointUpdateMsg : ROSBridgeMsg
        {
            public Vector3 position;
            public int id;

            public WaypointUpdateMsg(JSONNode msg)
            {
                float _x = float.Parse(msg["position"]["x"]);
                float _y = float.Parse(msg["position"]["y"]);
                float _z = float.Parse(msg["position"]["z"]);

                position = new Vector3(_x, _y, _z);
                id = int.Parse(msg["id"]);
            }

            public WaypointUpdateMsg(float x, float y, float z)
            {
                position = new Vector3(x, y, z);
            }
        }
    }
}
