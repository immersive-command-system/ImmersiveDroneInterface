using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;
using ISAACS;

public class ROSSensorConnection : MonoBehaviour {

    [Header("Sensor Variables")]
    public string sensorIP = "192.168.1.73";
    public int pointCloudLevel;
    public bool connectionStatus = false;

    [Header("Init with Subscribers")]
    public bool surfacePointCloud = true;
    public bool colorizedPointCloud3 = false;
    public bool surfaceMeshSubscriber = false;


    private ROSBridgeWebSocketConnection rosSensorConnection = null;
    
    // Use this for initialization
    void Start () {

        // Establish connection with the sensor IP
        rosSensorConnection = new ROSBridgeWebSocketConnection("ws://" + sensorIP, 9090);

        if (surfacePointCloud)
        {
            rosSensorConnection.AddSubscriber(typeof(PointCloud2Subscriber));
        }

        if (colorizedPointCloud3)
        {
            rosSensorConnection.AddSubscriber(typeof(ColorizedCloud3Subscriber));
            pointCloudLevel = 3;
        }

        if (surfacePointCloud)
        {
            // TODO: Remove comments when adding MeshSubscriber
            //rosSensorConnection.AddSubscriber(typeof(SurfaceMeshSubscriber));
        }

        rosSensorConnection.Connect();
        Debug.Log("Sending connection attempt to ROS");
        connectionStatus = true;
    }

    // Update is called once per frame
    void Update () {
        rosSensorConnection.Render();	
	}

    private void resetLampConnection(System.Type subscriber)
    {
        if (rosSensorConnection != null)
        {
            rosSensorConnection.Disconnect();
        }

        //lamp_ros_variable = new ROSBridgeWebSocketConnection("ws://"+LampIP, 9090);
        rosSensorConnection.AddSubscriber(subscriber);
        rosSensorConnection.Connect();
    }

    public void LampSubscribe_SurfacePointcloud()
    {
        resetLampConnection(typeof(PointCloud2Subscriber));
        pointCloudLevel = -1;
    }

    public void LampSubscribe_Colorized_0()
    {
        resetLampConnection(typeof(ColorizedCloud0Subscriber));
        pointCloudLevel = 0;
    }

    public void LampSubscribe_Colorized_1()
    {
        resetLampConnection(typeof(ColorizedCloud1Subscriber));
        pointCloudLevel = 1;
    }

    public void LampSubscribe_Colorized_2()
    {
        resetLampConnection(typeof(ColorizedCloud2Subscriber));
        //pointCloudLevel = 2;
        pointCloudLevel = 0;

    }

    public void LampSubscribe_Colorized_3()
    {
        resetLampConnection(typeof(ColorizedCloud3Subscriber));
        //pointCloudLevel = 3;
        pointCloudLevel = 0;
    }

    public void LampSubscribe_Colorized_4()
    {
        resetLampConnection(typeof(ColorizedCloud4Subscriber));
        //pointCloudLevel = 4;
        pointCloudLevel = 3;
    }

    public void LampSubscribe_Colorized_5()
    {
        resetLampConnection(typeof(ColorizedCloud5Subscriber));
        pointCloudLevel = 5;
    }

    // Functions for ROS begin here.
    public bool GetConnectionStatus()
    {
        return connectionStatus;
    }



    /// <summary>
    /// Extremely important to disconnect from ROS. OTherwise packets continue to flow
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting from ROS");
        if ( rosSensorConnection != null)
        {
            rosSensorConnection.Disconnect();
        }

    }

    // Tool to get your local ip address.
    public static class IPManager
    {
        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
    }

}
