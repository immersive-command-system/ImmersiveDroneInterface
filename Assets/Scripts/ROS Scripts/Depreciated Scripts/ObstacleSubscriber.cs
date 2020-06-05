﻿using ISAACS;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;

public class ObstacleSubscriber : ROSBridgeSubscriber
{
    private static int numberThing = 0;


    public static string GetMessageTopic()
    {
        return "/vis/true_env";
    }

    public static string GetMessageType()
    {
        return "visualization_msgs/Marker";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new ObstacleMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("callback");
        Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
        ObstacleMsg pose = (ObstacleMsg)msg;
        if (!WorldProperties.obstacleids.Contains(pose.id) && pose.id != 0)
        {
            WorldProperties.obstacleids.Add(pose.id);
            GameObject world = GameObject.FindWithTag("World");
            
            GameObject torus = (GameObject)WorldProperties.worldObject.GetComponent<WorldProperties>().torus;
            GameObject newTorus = Object.Instantiate(torus);

          

            //newTorus.name = pose.id + "";
            newTorus.transform.parent = world.transform;
            newTorus.transform.localPosition = new Vector3(-pose._x, pose._z + tablePos.z + 0.148f, -pose._y);
            newTorus.transform.localScale = new Vector3(pose.scale_x, pose.scale_x, pose.scale_x) * 5;
            WorldProperties.obstacles.Add(newTorus);
/*
            var radius = newTorus.GetComponent<MeshFilter>().mesh.bounds.extents.x;
            Vector3 start = new Vector3(newTorus.transform.localPosition.x, newTorus.transform.localPosition.y - radius, newTorus.transform.localPosition.z);
            Vector3 end = new Vector3(newTorus.transform.localPosition.x, tablePos.z, newTorus.transform.localPosition.z);
            Vector3 offset = start - end;
            var scale = new Vector3(1, offset.magnitude / 2, 1);
            var position = start + (offset / 2);

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.localPosition = position;
            pole.transform.up = offset;
            pole.transform.localScale = scale;*/

            //Debug.Log("making torus id: " + newTorus.name);

        }
    }

}
