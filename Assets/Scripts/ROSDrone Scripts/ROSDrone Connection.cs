using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;

public class ROSDroneConnection : MonoBehaviour {
    private ROSBridgeWebSocketConnection ros = null;

    void Start()
    {
        ros = new ROSBridgeWebSocketConnection("192.168.10.4", 9090);
        ros.AddSubscriber(typeof(ROSBridgeSubscriber));
        ros.AddServiceResponse(typeof(RosDroneServiceResponse));
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
