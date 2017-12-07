using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class ROSDroneSubscriber : ROSBridgeSubscriber
{

    public static string GetMessageTopic()
    {
        return "/state/dubins";
    }

    public static string GetMessageType()
    {
        return "crazyflie_msgs/DubinsStateStamped";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new DronePositionMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("callback");
        GameObject robot = GameObject.FindWithTag("ROSDrone");
        if (robot == null)
            Debug.Log("Can't find the robot???");
        else
        {
            DronePositionMsg pose = (DronePositionMsg)msg;
            Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
            robot.transform.localPosition = new Vector3(pose._x / 5 + tablePos.x, pose._z / 5 + tablePos.z + 2.1f, pose._y / 5);
            //Debug.Log(robot.transform.position);
            //robot.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
        }
    }
}