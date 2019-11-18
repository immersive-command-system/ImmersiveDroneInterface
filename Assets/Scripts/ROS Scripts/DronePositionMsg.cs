using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

using ROSBridgeLib.interface_msgs;
using UnityEngine;
using ISAACS;
using System.IO;
using UnityEditor;

namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class DronePositionMsg : ROSBridgeMsg
        {
            public float _x, _y, _z;

            public DronePositionMsg(JSONNode msg)
            {
                _x = (float.Parse(msg["latitude"])) ;
                _y = (float.Parse(msg["altitude"])) - 48.0f + 4.0f;
                _z = (float.Parse(msg["longitude"])) ;

                //_x = float.Parse(msg["state"]["x"]);
                //_y = float.Parse(msg["state"]["y"]);
                //_z = float.Parse(msg["state"]["z"]);
            }

            public DronePositionMsg(float x, float y, float z)
            {
                _x = x;
                _y = y;
                _z = z;
            }
        }
    }
}
