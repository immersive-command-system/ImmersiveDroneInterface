using ISAACS;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;

public class EnvironmentSubscriber : ROSBridgeSubscriber
{
    public static string GetMessageTopic()
    {
        return "/tf";
    }

    public static string GetMessageType()
    {
        return "tf2_msgs/TFMessage";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new EnvironmentMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        EnvironmentMsg poseMsg = (EnvironmentMsg)msg;
        
        if (poseMsg.id_.Contains("hoop"))
        {
            char numID = poseMsg.id_[poseMsg.id_.Length - 1];
            //Debug.Log("Got a tf message: " + poseMsg.id_[poseMsg.id_.Length - 1]);

            GameObject currentHoop = null;

            if (!WorldProperties.hoopsDict.ContainsKey(numID))
            {
                GameObject world = GameObject.FindWithTag("World");
                currentHoop = Object.Instantiate(world.GetComponent<WorldProperties>().torus);
                currentHoop.transform.parent = world.transform;
                WorldProperties.hoopsDict[numID] = currentHoop;
                Debug.Log("Made hoop with id: " + numID);
            }
            else
            {
                currentHoop = WorldProperties.hoopsDict[numID];
            }

            currentHoop.transform.localPosition = WorldProperties.RosSpaceToWorldSpace(poseMsg.x_, poseMsg.y_, poseMsg.z_) +
                                                    WorldProperties.torusModelOffset;
            currentHoop.transform.localRotation = new Quaternion(poseMsg.x_rot_+1, poseMsg.y_rot_, poseMsg.z_rot_, poseMsg.w_rot_);
        }
    }
}
