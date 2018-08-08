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
        cart.transform.localPosition = WorldProperties.RosSpaceToWorldSpace(pose._x+0.5f, pose._y+0.1f, pose._z - 0.95f);
        cart.transform.localRotation = Quaternion.AngleAxis(WorldProperties.RosRotationToWorldYaw(pose._x_rot, pose._y_rot, pose._z_rot, pose._w_rot), Vector3.up);
    }
}
