using ISAACS;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;

public class ObstacleSubscriber : ROSBridgeSubscriber
{
    public static HashSet<int> ids = new HashSet<int>();
    public static GameObject torus = (GameObject)WorldProperties.worldObject.GetComponent<WorldProperties>().torus;
    public static string GetMessageTopic()
    {
        return "/vis/true_env";
    }

    public static string GetMessageType()
    {
        return "visualization_msgs/Marker";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new ObstacleMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("callback");
        Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
        ObstacleMsg pose = (ObstacleMsg)msg;
        if (!ids.Contains(pose.id) && pose.id != 0)
        {
            Debug.Log("making sphere id:" + pose.id);
            ids.Add(pose.id);
            GameObject world = GameObject.FindWithTag("World");
            Object.Instantiate(torus);
            torus.transform.parent = world.transform;
            torus.transform.localPosition = new Vector3(-pose._x, pose._z + tablePos.z + 0.148f, -pose._y);
            torus.transform.localScale = new Vector3(pose.scale_x, pose.scale_x, pose.scale_x) / 5;
        }
    }
}