using SimpleJSON;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class WaypointUpdateMsg : ROSBridgeMsg
        {
            public double[] position;

            public WaypointUpdateMsg(JSONNode msg)
            {
                double _x = double.Parse(msg["position"]["x"]);
                double _y = double.Parse(msg["position"]["y"]);
                double _z = double.Parse(msg["position"]["z"]);

                position = new double[3];
                position[0] = _x;
                position[1] = _y;
                position[2] = _z;
            }

            public WaypointUpdateMsg(float x, float y, float z)
            {
                position = new double[3];
                position[0] = (double) x;
                position[1] = (double) y;
                position[2] = (double) z;
            }
        }
    }
}
