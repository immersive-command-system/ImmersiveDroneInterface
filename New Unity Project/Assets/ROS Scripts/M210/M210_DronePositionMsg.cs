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
        public class M210_DronePositionMsg : ROSBridgeMsg
        {
            public float _lat, _long, _altitude;

            public M210_DronePositionMsg(JSONNode msg)
            {
                _lat = (float.Parse(msg["latitude"])); // position X
                _altitude = (float.Parse(msg["altitude"])); // position Y
                _long = (float.Parse(msg["longitude"])); // position Z
            }

            public M210_DronePositionMsg(float x, float y, float z)
            {
                _lat = x;
                _long = y;
                _altitude = z;
            }
        }
    }
}