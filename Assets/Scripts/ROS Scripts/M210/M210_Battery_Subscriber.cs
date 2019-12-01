using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.sensor_msgs;
using SimpleJSON;
using System.IO;
using UnityEditor;
using ISAACS;

public class M210_Battery_Subscriber : MonoBehaviour {


    public static Vector3 initialPosition = Vector3.zero;

    public new static string GetMessageTopic()
    {
        return "/dji_sdk/battery_state";
    }

    public new static string GetMessageType()
    {
        return "sensor_msgs/BatteryState";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new BatteryStateMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {

        BatteryStateMsg batteryState = (BatteryStateMsg)msg;
        float batteryPercentage = batteryState.GetBatteryPercentage();


        /**
        GameObject robot = GameObject.FindWithTag("Drone");
        GameObject drone = WorldProperties.selectedDrone.gameObjectPointer;

        if (drone != null)
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





//Debug.Log("Drone Position Callback");


