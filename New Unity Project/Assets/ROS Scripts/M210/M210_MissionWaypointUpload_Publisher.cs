using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class M210_MissionWaypointUpload_Publisher : MonoBehaviour
{

    public static string GetMessageTopic()
    {
        return "/dji_sdk/mission_waypoint_upload";
    }

    public static string GetMessageType()
    {
        return "dji_sdk/MissionWayPointTask";
    }


    public static string ToYAMLString(MissionWaypointTaskMsg msg)
    {
        return msg.ToYAMLString();
    }
}

