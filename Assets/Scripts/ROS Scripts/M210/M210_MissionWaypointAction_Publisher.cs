using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class M210_MissionWaypointAction_Publisher : MonoBehaviour
{

    public static string GetMessageTopic()
    {
        return "/dji_sdk/mission_waypoint_action";
    }

    public static string GetMessageType()
    {
        return "dji_sdk/MissionWaypointAction";
    }


    public static string ToYAMLString(MissionWaypointActionMsg msg)
    {
        return msg.ToYAMLString();
    }
}
