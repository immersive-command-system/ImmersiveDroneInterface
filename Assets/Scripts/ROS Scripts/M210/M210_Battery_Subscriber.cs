using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.sensor_msgs;
using SimpleJSON;
using System.IO;
using UnityEditor;
using ISAACS;
/// <summary>
/// Subscriber for the M210's battery level. We store the battery level and display it 
/// in the Unity user interface. If the level drops below a given threshold, we warn the user
/// to end the mission and return to home soon.
/// </summary>
public class M210_Battery_Subscriber : MonoBehaviour {

    public static Vector3 initialPosition = Vector3.zero;
    /// <summary>
    /// Returns the name of the ROS topic to subscribe to on the Manifold.
    /// </summary>
    public new static string GetMessageTopic()
    {
        return "/dji_sdk/battery_state";
    }
    /// <summary>
    /// Returns the ROS message which specifies the data structure of the GPS health message.
    /// </summary>
    public new static string GetMessageType()
    {
        return "sensor_msgs/BatteryState";
    }
    /// <summary>
    /// Returns the ROS battery state message containing information from the given JSON object.
    /// </summary>
    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new DroneBatteryStateMsg(msg);
    }

    /// <summary> 
    /// CallBack is called every time the subscriber receives a new message. It updates the drone's battery level
    /// in the user interface so the user has an idea of remaining flight time. Upon dropping below a certain threshold,
    /// the user is warned to land the drone.
    /// </summary>
    public new static void CallBack(ROSBridgeMsg msg)
    {
        /// TODO: actually do what the callback comments say.
        // Float representing the current battery level.
        DroneBatteryStateMsg batteryState = (DroneBatteryStateMsg)msg;
        float batteryPercentage = batteryState.GetBatteryPercentage();

        /** TODO: everything
        GameObject robot = GameObject.FindWithTag("Drone");
        GameObject drone = WorldProperties.selectedDrone.gameObjectPointer;

        if (drone != null)
        {
            TODO:
            Update UI field. If level drops below ___, show a warning to the user recommending the user to
            land the drone.
        }
        else
        {
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }
        **/

    }

}





//Debug.Log("Drone Position Callback");


