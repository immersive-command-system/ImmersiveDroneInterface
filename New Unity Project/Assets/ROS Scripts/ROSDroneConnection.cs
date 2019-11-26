using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;


public class ROSDroneConnection : MonoBehaviour
{
    private ROSBridgeWebSocketConnection ros = null;
    public bool connectionStatus = false;

    void Start()
    {
        // This is the IP of the linux computer that is connected to the drone.  
        ros = new ROSBridgeWebSocketConnection("ws://192.168.50.48", 9090);

        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));

        // ISAACS V2
        ros.AddSubscriber(typeof(M210_DronePositionSubscriber));
        //ros.AddPublisher(typeof(M210_MissionWaypointUpload_Publisher));
        //ros.AddPublisher(typeof(M210_MissionWaypointAction_Publisher));

        ros.Connect();
        Debug.Log("Sending connection attempt to ROS");
        connectionStatus = true;
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
            GetAuthority();
        }

        if (Input.GetKeyUp("v"))
        {
            GetVersion();
        }

        if (Input.GetKeyUp("a"))
        {
            Activation();
        }

        if (Input.GetKeyUp("2"))
        {
            Spin();
        }

        if (Input.GetKeyUp("0"))
        {
            StopSpinning();
        }

        if (Input.GetKeyUp("3"))
        {
            Takeoff();
        }

        if (Input.GetKeyUp("4"))
        {
            Land();
        }

        if (Input.GetKeyUp("p"))
        {

            uint[] command_list = new uint[16];
            uint[] command_params = new uint[16];
            for (int i = 0; i < 16; i++)
            {
                command_list[i] = 0;
                command_params[i] = 0;
            }

            MissionWaypointMsg test_waypoint_1 = new MissionWaypointMsg(0.00f, 0.00f, 20.0f, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
            MissionWaypointMsg test_waypoint_2 = new MissionWaypointMsg(0.0005f, 0.0005f, 25.0f, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
            Debug.Log(test_waypoint_1.ToYAMLString());

            MissionWaypointMsg[] test_waypoint_array = new MissionWaypointMsg[] { test_waypoint_1, test_waypoint_2 };
            /*for (int i = 0; i < 2; i++)
            {
                Debug.Log(test_waypoint_array[i].ToString());
            }
            */
            MissionWaypointTaskMsg test_Task = new MissionWaypointTaskMsg(15.0f, 15.0f, MissionWaypointTaskMsg.ActionOnFinish.RETURN_TO_HOME, 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.COORDINATED, MissionWaypointTaskMsg.ActionOnRCLost.FREE, MissionWaypointTaskMsg.GimbalPitchMode.FREE, test_waypoint_array);

            UploadMission(test_Task);
        }

        if (Input.GetKeyUp("e"))
        {
            ExecuteMission();
        }

        if (Input.GetKeyUp("i"))
        {
            InfoMission();
        }

    }

    // Functions for ROS
    public bool GetConnectionStatus()
    {
        return connectionStatus;
    }


    public void GetAuthority()
    {
        print("Get Authority");
        string service_name = "/dji_sdk/sdk_control_authority";
        ros.CallService(service_name, "[1]");
    }

    public void GetVersion()
    {
        print("version");
        string service_name = "/dji_sdk/query_drone_version";
        ros.CallService(service_name, "[1]");
    }

    public void Activation()
    {
        print("active");
        string service_name = "/dji_sdk/activation";
        ros.CallService(service_name, string.Format("[{0}]", 1));
    }

    public void Spin()
    {
        print("Spin");
        string service_name = "/dji_sdk/drone_arm_control";
        ros.CallService(service_name, "[1]");
    }

    public void StopSpinning()
    {
        print("Stop spinning");
        string service_name = "/dji_sdk/drone_arm_control";
        ros.CallService(service_name, "[0]");
    }

    public void Takeoff()
    {
        print("Takeoff");
        string service_name = "/dji_sdk/drone_task_control";
        ros.CallService(service_name, "[4]");
    }

    public void Land()
    {
        print("Land");
        string service_name = "/dji_sdk/drone_task_control";
        ros.CallService(service_name, "[6]");
    }

    public void UploadMission(MissionWaypointTaskMsg Task)
    {
        print("Upload Test Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_upload";
        Debug.Log(Task.ToYAMLString());
        ros.CallService(service_name, string.Format("[{0}]", Task.ToYAMLString())); // try with and without []
    }

    public void ExecuteMission()
    {
        print("Execute test Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_action";
        ros.CallService(service_name, "[0]");
    }

    public void InfoMission()
    {
        print("get info for Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_getInfo";
        ros.CallService(service_name, "[0]");
    }

    public void GoHome()
    {
        print("Going Home");
        string service_name = "/dji_sdk/drone_task_control";
        ros.CallService(service_name, "[1]");
    }




    public void SendServiceCall(string service, string args)
    {
        Debug.Log("Calling service: " + service);
        ros.CallService(service, args);
    }

}

