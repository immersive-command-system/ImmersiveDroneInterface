using SimpleJSON;
using System.Text;
using UnityEngine;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class WaypointUpdateMsg : ROSBridgeMsg
        {
            public double[] position;
            private const float table_height = 0.976f;

            public WaypointUpdateMsg(JSONNode msg)
            {
                double _x = double.Parse(msg["position"]["x"]);
                double _y = double.Parse(msg["position"]["y"]);
                double _z = double.Parse(msg["position"]["z"]);

                position = new double[3];
                position[0] = _x;
                position[1] = -_z;
                position[2] = (_y - 0.148f) - table_height;
            }

            public WaypointUpdateMsg(float x, float y, float z)
            {
                position = new double[3];

                Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
                position[0] = (double) x;
                position[1] = (double) -z;
                position[2] = (double) (y - 0.148f);
            }

            public override string ToYAMLString()
            {
                return JsonUtility.ToJson(this);
            }
        }
    }
}
