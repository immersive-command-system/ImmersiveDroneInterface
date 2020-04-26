using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using SimpleJSON;
using System.IO;
using UnityEditor;
using ISAACS;
/// <summary>
/// Subscriber for the M210's GPS health (signal strength). We store the updated health, and display it 
/// in the Unity user interface. If the signal strenght is too low, the drone shouldn't be allowed to fly.
/// </summary>
public class M210_GPSHealth_Subscriber : MonoBehaviour {
    /// <summary>
    /// Returns the name of the ROS topic to subscribe to on the Manifold.
    /// </summary>
    public new static string GetMessageTopic()
    {
        return "/dji_sdk/gps_health";
    }
    /// <summary>
    /// Returns the ROS message which specifies the data structure of the GPS health message.
    /// </summary>
    public new static string GetMessageType()
    {
        return "std_msgs/UInt8";
    }
    /// <summary>
    /// Returns the ROS GPS health message containing information from the given JSON object.
    /// </summary>
    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new GPSHealthMsg(msg);
    }

    /// <summary> 
    /// CallBack is called every time the subscriber receives a new message. It updates the drone's GPS health
    /// in the user interface so the user can see it is still safe to fly the drone.
    /// </summary>
    public new static void CallBack(ROSBridgeMsg msg)
    {
        GPSHealthMsg gpsHealth = (GPSHealthMsg)msg;
        float gpsHealthPercent = gpsHealth.GeGPSHealthPercentage();
      /* TODO: Update global variable storing the drone's current GPS health, and display it in the UI.

        GameObject robot = GameObject.FindWithTag("Drone");
        GameObject drone = WorldProperties.selectedDrone.gameObjectPointer;

        if (drone != null)
        {
            TODO:
            Update global variable storing the drone's current GPS health, and display it in the UI.
        }
        else
        {
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }
        **/
    }
}
