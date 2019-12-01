using System.Collections;
using System.Text;
using SimpleJSON;
using ROSBridgeLib.std_msgs;

using ROSBridgeLib.interface_msgs;
using UnityEngine;
using System.IO;
using UnityEditor;
using ISAACS;

namespace ROSBridgeLib
{
    namespace std_msgs
    {
        public class GPSHealthMsg : ROSBridgeMsg
        {

            public float gpsHealth;

            public GPSHealthMsg(JSONNode msg)
            {
                gpsHealth = (float.Parse(msg["data"])); // position X
            }

            public float GeGPSHealthPercentage()
            {
                return gpsHealth*20;
            }
        }
    }
}