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
        return "/pose/cart";
    }

    public static string GetMessageType()
    {
        return "geometry_msgs/PoseStamped";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new EnvironmentMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("Cart Position Callback");

        GameObject cart = null;

        if (GameObject.FindGameObjectsWithTag("Cart").Length == 0)
        {
            GameObject world = GameObject.FindWithTag("World");
            cart = Object.Instantiate(world.GetComponent<WorldProperties>().cart);
            cart.transform.parent = world.transform;
            Debug.Log("Made cart");
        } else
        {
            cart = GameObject.FindWithTag("Cart");
        }

        EnvironmentMsg pose = (EnvironmentMsg)msg;
        cart.transform.localPosition = WorldProperties.RosSpaceToWorldSpace(pose._x, pose._y, pose._z);
        //cart.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
    }
}
