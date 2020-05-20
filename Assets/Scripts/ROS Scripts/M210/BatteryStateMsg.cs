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
    namespace sensor_msgs
    {

        public class DroneBatteryStateMsg : ROSBridgeMsg
        {
            public float battery_percentage;

            public DroneBatteryStateMsg(JSONNode msg)
            {
                battery_percentage = (float.Parse(msg["percentage"]));
            }

            public float GetBatteryPercentage()
            {
                return battery_percentage;
            }
        }
    }
}