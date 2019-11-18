using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;
using ISAACS;

public class ROSDroneConnection : MonoBehaviour
{
    private ROSBridgeWebSocketConnection ros = null;

    void Start()
    {
        // This is the IP of the linux computer that is connected to the drone.  
        ros = new ROSBridgeWebSocketConnection("ws://192.168.50.48", 9090);

        // ISAACS V1

        ros.AddSubscriber(typeof(ObstacleSubscriber));
        ros.AddSubscriber(typeof(EnvironmentSubscriber));
        ros.AddSubscriber(typeof(DronePositionSubscriber));
        ros.AddPublisher(typeof(UserpointPublisher));
        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));

        // ISAACS V2
        ros.AddSubscriber(typeof(M210_DronePositionSubscriber));
        //ros.AddPublisher(typeof(M210_MissionWaypointUpload_Publisher));
        //ros.AddPublisher(typeof(M210_MissionWaypointAction_Publisher));

        ros.Connect();
        Debug.Log("Sending connection attempt to ROS");
    }

    // Extremely important to disconnect from ROS. OTherwise packets continue to flow
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

        if (Input.GetKeyUp("1"))
        {
            print("Get Auth");
            string service_name = "/dji_sdk/sdk_control_authority";
            ros.CallService(service_name, "[1]");
        }


        if (Input.GetKeyUp("v"))
        {
            print("version");
            string service_name = "/dji_sdk/query_drone_version";
            ros.CallService(service_name, "[1]");
        }

        if (Input.GetKeyUp("a"))
        {
            print("active");
            string service_name = "/dji_sdk/activation";
            ros.CallService(service_name, string.Format("[{0}]", 1));
        }

        if (Input.GetKeyUp("2"))
        {
            print("Spin");
            string service_name = "/dji_sdk/drone_arm_control";
            ros.CallService(service_name,  "[1]");
        }
        if (Input.GetKeyUp("0"))
        {
            print("Stop spinning");
            string service_name = "/dji_sdk/drone_arm_control";
            ros.CallService(service_name, "[0]");
        }


        if (Input.GetKeyUp("3"))
        {
            print("Takeoff");
            string service_name = "/dji_sdk/drone_task_control";
            ros.CallService(service_name, "[4]");
        }

        if (Input.GetKeyUp("4"))
        {
            print("Land");
            string service_name = "/dji_sdk/drone_task_control";
            ros.CallService(service_name, "[6]");
        }


    }


    public void PublishWaypointUpdateMessage(UserpointInstruction msg)
    {
        Debug.Log("Published new userpoint instruction: "+ msg.ToYAMLString());
        ros.Publish(UserpointPublisher.GetMessageTopic(), msg);
    }

    public void SendServiceCall(string service, string args)
    {
        Debug.Log("Calling service: " + service);
        ros.CallService(service, args);
    }

}

