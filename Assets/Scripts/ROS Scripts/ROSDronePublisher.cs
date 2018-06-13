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
        return "meta_planner_msgs/UserpointInstruction.msg";
    }

    public static string ToYAMLString(UserpointInstruction msg)
    {
        return msg.ToYAMLString();
    }
}

// To publish to this topic:

// UserpointInstruction msg = new UserpointInstruction(curr_id, prev_id, x, y, z, action); 
// ros.Publish(ROSDronePublisher.GetMessageTopic(), msg);