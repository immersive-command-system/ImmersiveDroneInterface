using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

using ROSBridgeLib.interface_msgs;
using UnityEngine;
using System.IO;
using UnityEditor;


namespace ROSBridgeLib
{
    namespace interface_msgs
    {
        public class M210_DronePositionMsgLocal : ROSBridgeMsg
        {
            public float _x, _y, _z;

            public M210_DronePositionMsgLocal(JSONNode msg)
            {
                _x = (float.Parse(msg["x"])); // position X
                _y = (float.Parse(msg["y"])); // position Y
                _z = (float.Parse(msg["z"])); // position Z
            }

            public M210_DronePositionMsgLocal(float x, float y, float z)
            {
                _x = x;
                _y = y;
                _z = z;
            }
        }
    }
}