using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

/**
 * This is a toy example of the Unity-ROS interface talking to the TurtleSim 
 * tutorial (circa Groovy). Note that due to some changes since then this will have
 * to be slightly re-written, but as its a test ....
 * 
 * This defines the callback that links the pose message. It moves the Dalek with
 * the turtlesim
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.0
 **/

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
        return new PoseMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("callback");
        GameObject robot = GameObject.FindWithTag("ROSDrone");
        if (robot == null)
            Debug.Log("Can't find the robot???");
        else
        {
            PoseMsg pose = (PoseMsg)msg;
            Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
            robot.transform.position = new Vector3(pose.getX()/5 + tablePos.x, pose.getZ()/5 + tablePos.z +2.1f, pose.getY()/5);
            Debug.Log(robot.transform.position);
            //robot.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
        }
    }
}