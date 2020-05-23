using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using SimpleJSON;
using System.Text;

public class ColorizedCloud3Subscriber : ROSBridgeSubscriber
{
    public static string rendererObjectName = "PlacementPlane"; // pick a center point of the map, ideally as part of rotating map

    private static bool verbose = false;

    public new static string GetMessageTopic()
    {
        return "/colorized_points_3";
    }

    public new static string GetMessageType()
    {
        return "sensor_msgs/PointCloud2";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new PointCloud2Msg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        PointCloud2Msg pointCloudMsg = (PointCloud2Msg)msg;

        if (verbose)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(pointCloudMsg.GetHeader().GetSeq());
            sb.Append(":\n");
            sb.AppendFormat("Size: {0} X {1} = {2}\n", pointCloudMsg.GetWidth(), pointCloudMsg.GetHeight(), pointCloudMsg.GetWidth() * pointCloudMsg.GetHeight());
            sb.Append(pointCloudMsg.GetFieldString());
            Debug.Log(sb.ToString());
        }

        PointCloudVisualizer visualizer = GameObject.Find(rendererObjectName).GetComponent<PointCloudVisualizer>();
        visualizer.SetPointCloud(pointCloudMsg.GetCloud());
        Debug.Log("Updated Point Cloud");
    }
}
