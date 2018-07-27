using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using ISAACS;

public class ROSDroneSubscriber : ROSBridgeSubscriber
{
    public new static string GetMessageTopic()
    {
        return "/state/position_velocity";
    }

    public new static string GetMessageType()
    {
        return "crazyflie_msgs/PositionVelocityStateStamped";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new DronePositionMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("Drone Position Callback");

        GameObject robot = GameObject.FindWithTag("Drone");
        if (robot != null)
        {
            DronePositionMsg pose = (DronePositionMsg)msg;
            robot.transform.localPosition = WorldProperties.RosSpaceToWorldSpace(pose._x, pose._y, pose._z);
            //robot.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
        } else {
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }
    }
}