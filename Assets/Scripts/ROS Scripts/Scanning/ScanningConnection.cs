using ROSBridgeLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanningConnection : MonoBehaviour {

    private ROSBridgeWebSocketConnection ros = null;

    // Use this for initialization
    void Start () {
        // TODO: Understand this line
        Debug.Log("Starting Scanning Connection...");
        ros = new ROSBridgeWebSocketConnection("ws://128.32.43.94", 9090);
        ros.AddSubscriber(typeof(PointCloud2Subscriber));
        ros.Connect();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting from ROS");
        if (ros != null)
        {
            ros.Disconnect();
        }
    }

    // Update is called once per frame in Unity
    void Update()
    {
        ros.Render();
    }
}
