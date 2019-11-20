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

public class M210_DronePositionSubscriber : MonoBehaviour {

    public new static string GetMessageTopic()
    {
        return "/dji_sdk/gps_position";
    }

    public new static string GetMessageType()
    {
        return "sensor_msgs/NavSatFix";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new M210_DronePositionMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("Drone Position Callback");

        //GameObject robot = GameObject.FindWithTag("Drone");
        GameObject cube = GameObject.FindWithTag("test_drone");

        if (cube != null)
        {
            M210_DronePositionMsg pose = (M210_DronePositionMsg)msg;
            cube.transform.position = M210_RosSpaceToWorld(pose._lat, pose._long, pose._altitude); //TODO
        }
        else
        {
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }
    }


    public static Vector3 M210_RosSpaceToWorld(float _lat , float _long, float _altitude)
    {
        // NOTE: Assumed the earth is a sphere to greatly simplify the math (it's elliptical, so we might be losing some precision. TODO: test)
        // Conversion is from angles to meters
        // See https://gssc.esa.int/navipedia/index.php/Ellipsoidal_and_Cartesian_Coordinates_Conversion
        // and https://en.wikipedia.org/wiki/Geographic_coordinate_system#/media/File:ECEF.svg
        // TODO: is y and z inverted?

        float earth_radius = 6378137;

        float lat_rad = Mathf.PI * _lat / 180;
        float long_rad = Mathf.PI * _long / 180;
        float alt_rad = Mathf.PI * _altitude / 180;

        float x_pos = (earth_radius + alt_rad) * (float)Math.Cos(lat_rad) * (float)Math.Cos(long_rad);
        float y_pos = (earth_radius + alt_rad) * (float)Math.Cos(lat_rad) * (float)Math.Sin(long_rad);
        float z_pos = (earth_radius + alt_rad) * (float)Math.Sin(lat_rad);

        return new Vector3(x_pos, y_pos,z_pos);
    }

    /// <summary>
    /// Finds and keeps track of all the closest obstacle distancecs as strings in a list 
    /// </summary>
    static void SaveData()
    {
        Debug.Log("TODO: Save Data");
    }



}
