using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class ROSDronePublisher : ROSBridgePublisher
{

    public static string GetMessageTopic()
    {
        return "/waypoints";
    }

    public static string GetMessageType()
    {
        return "meta_planner_msgs/WaypointUpdateMsg";
    }

    public static string ToYAMLString(WaypointUpdateMsg msg)
    {
        return msg.ToYAMLString();
    }
}

// To publish to this topic:

// WaypointUpdateMsg msg = new WaypointUpdateMsg(x, y, z); 
// ros.Publish(ROSDronePublisher.GetMessageTopic(), msg);