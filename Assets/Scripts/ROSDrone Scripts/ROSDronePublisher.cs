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
        return "interface_msgs/WaypointUpdateMsg";
    }

    public static string ToYAMLString(WaypointUpdateMsg msg)
    {
        return msg.ToYAMLString();
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new WaypointUpdateMsg(msg);
    }
}

// To publish to this topic:

// WaypointUpdateMsg msg = new WaypointUpdateMsg(x, y, z); 
// ros.Publish(ROSDronePublisher.GetMessageTopic(), msg);