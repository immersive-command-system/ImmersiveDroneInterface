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
        public class M210_DronePositionMsg : ROSBridgeMsg
        {
            public float _lat, _long, _altitude;

            public M210_DronePositionMsg(JSONNode msg)
            {
                _lat = (float.Parse(msg["latitude"]));
                _long = (float.Parse(msg["altitude"]));
                _altitude = (float.Parse(msg["longitude"]));
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