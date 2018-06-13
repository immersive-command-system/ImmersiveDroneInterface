using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class ROSDroneSubscriber : ROSBridgeSubscriber
{
    public static string GetMessageTopic()
    {
        return "/state/position_velocity";
    }

    public static string GetMessageType()
    {
        return "crazyflie_msgs/PositionVelocityStateStamped";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new DronePositionMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("callback");
        GameObject robot = GameObject.FindWithTag("Drone");
        if (robot == null)
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        else
        {
            DronePositionMsg pose = (DronePositionMsg)msg;
            Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
            robot.transform.localPosition = new Vector3(pose._x, pose._z + tablePos.z + 0.148f, -pose._y);
            //Debug.Log(robot.transform.position);
            //robot.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
        }
    }
}