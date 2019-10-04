using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using ISAACS;
using System.IO;
using UnityEditor;

public class TrajectoryBoundSubscriber : ROSBridgeSubscriber
{

    public new static string GetMessageTopic()
    {
        return "/vis/bound";
    }

    public new static string GetMessageType()
    {
        //New message type needed
        return "crazyflie_msgs/PositionVelocityStateStamped";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new TrajectoryBoundMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("Bounding box callback");
        GameObject drone = GameObject.FindWithTag("Drone");
        if (drone != null)
        {
            //Get waypoints, get lines from waypoints, apply box from msg radius
        }
        else
        {
            Debug.Log("Can't find drone");
        }
    }
  

}