using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;

public class ROSDroneConnection : MonoBehaviour {
    private ROSBridgeWebSocketConnection ros = null;

    void Start()
    {
        Debug.Log("Attempting to create ROS connection");
        ros = new ROSBridgeWebSocketConnection("ws://192.168.0.107", 9090);
        ros.AddSubscriber(typeof(ROSDroneSubscriber));
        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));
        ros.Connect();
    }

    // Extremely important to disconnect from ROS. OTherwise packets continue to flow
    void OnApplicationQuit()
    {
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
