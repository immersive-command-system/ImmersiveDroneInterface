using ROSBridgeLib;
using ROSBridgeLib.sensor_msgs;
using SimpleJSON;
using UnityEngine;

public class PointCloud2Subscriber : ROSBridgeSubscriber
{

    public new static string GetMessageTopic()
    {
        return "velodyne/velodyne_points";
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
        Debug.Log(pointCloudMsg.GetHeader().GetSeq());
    }
}
