using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class UserpointPublisher : ROSBridgePublisher
{

    public static string GetMessageTopic()
    {
        return "/dji_sdk/";
    }

    public static string GetMessageType()
    {
        return "sensor_msgs/NavSatFix";
    }
    

    public static string ToYAMLString(UserpointInstruction msg)
    {
        return msg.ToYAMLString();
    }
}

// To publish to this topic:

// UserpointInstruction msg = new UserpointInstruction(curr_id, prev_id, x, y, z, action); 
// ros.Publish(ROSDronePublisher.GetMessageTopic(), msg);