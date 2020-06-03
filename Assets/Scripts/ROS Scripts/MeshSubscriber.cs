using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using ROSBridgeLib.voxblox_msgs;

public class MeshSubscriber : ROSBridgeSubscriber
{
    public static string rendererObjectName = "PlacementPlane"; // pick a center point of the map, ideally as part of rotating map


    public new static string GetMessageTopic()
    {
        return "/voxblox_node/mesh";
    }

    public new static string GetMessageType()
    {
        return "voxblox_msgs/Mesh";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new ROSBridgeLib.voxblox_msgs.MeshMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("Mesh Visualizer Callback.");
        MeshMsg meshMsg = (MeshMsg)msg;
        MeshVisualizer visualizer = GameObject.Find(rendererObjectName).GetComponent<MeshVisualizer>();
        visualizer.SetMesh(meshMsg);
    }
}