using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;

public class ROSDroneConnection : MonoBehaviour {
    private ROSBridgeWebSocketConnection ros = null;

    void Start()
    {
        Debug.Log("Creating ROS connection");
        ros = new ROSBridgeWebSocketConnection("ws://192.168.10.4", 9090);
        ros.AddSubscriber(typeof(ROSDroneSubscriber));
        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));
        ros.Connect();
        Debug.Log("ROS connected");
        //ros.callService ("/turtle1/set_pen", "{\"off\": 0}");
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
