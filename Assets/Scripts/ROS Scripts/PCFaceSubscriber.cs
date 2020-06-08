using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using ROSBridgeLib.rntools;

public class PCFaceSubscriber : ROSBridgeSubscriber
{
    public static string rendererObjectName = "PlacementPlane"; // pick a center point of the map, ideally as part of rotating map


    public new static string GetMessageTopic()
    {
        return "/colorized_points_faced_0";
    }

    public new static string GetMessageType()
    {
        return "rntools/PCFace";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new PCFaceMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("Mesh Visualizer Callback.");
        PCFaceMsg meshMsg = (PCFaceMsg)msg;
        PCFaceVisualizer visualizer = GameObject.Find(rendererObjectName).GetComponent<PCFaceVisualizer>();
        visualizer.SetMesh(meshMsg);
    }
}