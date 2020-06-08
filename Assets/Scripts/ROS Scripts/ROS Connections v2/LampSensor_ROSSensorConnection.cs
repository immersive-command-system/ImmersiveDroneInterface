using System;
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.sensor_msgs;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;

using ISAACS;

// TODO: Integrate Colour Logic

public class LampSensor_ROSSensorConnection : MonoBehaviour, ROSTopicSubscriber
{
    // Helper enum
    public enum PointCloudLevel
    {
        RED = 0,
        ORANGE = 1,
        YELLOW = 2,
        GREEN = 3,
        BLUE = 4,
        LIGHT_BLUE = 5,
        WHITE = 6
    }

    // Visualizer variables
    public static string rendererObjectName = "PlacementPlane"; // pick a center point of the map, ideally as part of rotating map
    public PointCloudLevel pointCloudLevel = PointCloudLevel.WHITE;

    // Private connection variables
    private ROSBridgeWebSocketConnection ros = null;
    public string client_id;

    // Initilize the sensor
    public void InitilizeSensor(int uniqueID, string sensorIP, int sensorPort, List<string> sensorSubscribers)
    {
        Debug.Log("Init LAMP Connection at IP " + sensorIP + " Port " + sensorPort.ToString());

        ros = new ROSBridgeWebSocketConnection("ws://" + sensorIP, sensorPort);
        client_id = uniqueID.ToString();

        foreach (string subscriber in sensorSubscribers)
        {
            string subscriberTopic = "";

            switch (subscriber)
            {
                case "surface_pointcloud":
                    subscriberTopic = "/voxblox_node/" + subscriber;
                    break;
                default:
                    subscriberTopic = "/" + subscriber;
                    break;
            }
            Debug.Log(" LAMP Subscribing to : " + subscriberTopic);
            ros.AddSubscriber(subscriberTopic, this);
        }

        Debug.Log("Lamp Connection Established");
        ros.Connect();
    }
    // Update is called once per frame in Unity
    void Update()
    {
        if (ros != null)
        {
            ros.Render();
        }
    }

    // ROS Topic Subscriber methods
    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        Debug.Log(" LAMP Recieved message");

        ROSBridgeMsg result = null;
        // Writing all code in here for now. May need to move out to separate handler functions when it gets too unwieldy.
        switch (topic)
        {
            case "/voxblox_node/surface_pointcloud":
                Debug.Log(" LAMP Recieved surface point cloud message");
                PointCloud2Msg pointCloudMsg = new PointCloud2Msg(raw_msg);
                PointCloudVisualizer(pointCloudMsg);
                break;
            default:
                Debug.LogError("Topic not implemented: " + topic);
                break;
        }
        return result;
    }
    public string GetMessageType(string topic)
    {
        Debug.Log("Point Cloud message type is returned as sensor_msg/PointCloud2 by default");
        return "sensor_msgs/PointCloud2";
        /**
        switch (topic)
        {
            case "/dji_sdk/attitude":
                return "geometry_msgs/QuaternionStamped";
            case "/dji_sdk/battery_state":
                return "sensor_msgs/BatteryState";
            case "/dji_sdk/flight_status":
                return "std_msgs/UInt8";
            case "/dji_sdk/gimbal_angle":
                return "geometry_msgs/Vector3Stamped";
            case "/dji_sdk/gps_health":
                return "std_msgs/UInt8";
            case "/dji_sdk/gps_position":
                return "sensor_msgs/NavSatFix";
            case "/dji_sdk/imu":
                return "sensor_msgs/Imu";
            case "/dji_sdk/rc":
                return "sensor_msgs/Joy";
            case "/dji_sdk/velocity":
                return "geometry_msgs/Vector3Stamped";
            case "/dji_sdk/height_above_takeoff":
                return "std_msgs/Float32";
            case "/dji_sdk/local_position":
                return "geometry_msgs/PointStamped";
        }
        Debug.LogError("Topic " + topic + " not registered.");
        return "";

        **/
    }

    // Visualizer helper scripts
    private void PointCloudVisualizer(PointCloud2Msg pointCloudMsg)
    {
        // Idea: We could have each sesnor have a PointCloudVisualizer attached to it and use that one.
        PointCloudVisualizer visualizer = GameObject.Find(rendererObjectName).GetComponent<PointCloudVisualizer>();
        visualizer.SetPointCloud(pointCloudMsg.GetCloud());
        Debug.Log("Updated Point Cloud");
    }

    /// <summary>
    /// Helper functions for subscribing to different LAMP topics.
    /// </summary>
    /// <returns></returns>
    private void resetLampConnection(System.Type subscriber)
    {
        if (ros != null)
        {
            ros.Disconnect();
        }

        ros.AddSubscriber(subscriber);
        ros.Connect();
    }
    public void LampSubscribe_SurfacePointcloud()
    {
        resetLampConnection(typeof(PointCloud2Subscriber));
        pointCloudLevel = PointCloudLevel.WHITE;
    }
    public void LampSubscribe_Colorized_0()
    {
        resetLampConnection(typeof(ColorizedCloud0Subscriber));
        pointCloudLevel = PointCloudLevel.RED;
    }
    public void LampSubscribe_Colorized_1()
    {
        resetLampConnection(typeof(ColorizedCloud1Subscriber));
        pointCloudLevel = PointCloudLevel.ORANGE;
    }
    public void LampSubscribe_Colorized_2()
    {
        resetLampConnection(typeof(ColorizedCloud2Subscriber));
        pointCloudLevel = PointCloudLevel.YELLOW;
    }
    public void LampSubscribe_Colorized_3()
    {
        resetLampConnection(typeof(ColorizedCloud3Subscriber));
        pointCloudLevel = PointCloudLevel.GREEN;
    }
    public void LampSubscribe_Colorized_4()
    {
        resetLampConnection(typeof(ColorizedCloud4Subscriber));
        pointCloudLevel = PointCloudLevel.BLUE;
    }
    public void LampSubscribe_Colorized_5()
    {
        resetLampConnection(typeof(ColorizedCloud5Subscriber));
        pointCloudLevel = PointCloudLevel.LIGHT_BLUE;
    }

}
