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
        if (!ids.Contains(pose.id))
        {
            Debug.Log("making sphere id:" + pose.id);
            ids.Add(pose.id);
            GameObject world = GameObject.FindWithTag("World");
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = world.transform;
            sphere.transform.localPosition = new Vector3(pose._x + tablePos.x, pose._z + tablePos.z + 0.148f, pose._y / 5);
            sphere.transform.localScale = new Vector3(pose.scale_x, pose.scale_x, pose.scale_x) / 5;
            //robot.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
            //string path = "Assets/Results/user_test.txt";

            ////Here we write the obstacle locations to a file.
            //StreamWriter writer = new StreamWriter(path, true);
            //writer.WriteLine("Sphere: "+ pose.id + ", Position: " + sphere.transform.localPosition + ", Scale: " + sphere.transform.localScale);
            //writer.Close();
        }
    }
}