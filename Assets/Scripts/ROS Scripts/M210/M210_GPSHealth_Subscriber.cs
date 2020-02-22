using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using SimpleJSON;
using System.IO;
using UnityEditor;
using ISAACS;

public class M210_GPSHealth_Subscriber : MonoBehaviour {

    public new static string GetMessageTopic()
    {
        return "/dji_sdk/gps_health";
    }

    public new static string GetMessageType()
    {
        return "std_msgs/UInt8";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new GPSHealthMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        GPSHealthMsg gpsHealth = (GPSHealthMsg)msg;
        float gpsHealthPercent = gpsHealth.GeGPSHealthPercentage();

        //Debug.Log("GPS Health Callback: " + gpsHealthPercent);

        /**
        GameObject robot = GameObject.FindWithTag("Drone");
        GameObject drone = WorldProperties.selectedDrone.gameObjectPointer;

        if (
        != null)
        {
            TODO:
            Update Drone Gameobject
        }
        else
        {
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }

        **/
    }
}
