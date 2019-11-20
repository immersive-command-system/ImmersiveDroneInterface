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

        if (Input.GetKeyUp("p"))
        {
            print("Upload Test Waypoint Mission");
            string service_name = "/dji_sdk/mission_waypoint_upload";

            // Initial Sim La, Long, Atl, etc can be set on the Windows Pc
            // Can also use the GPS stream data to get initial values

            uint[] command_list = new uint[16];
            uint[] command_params = new uint[16];
            for (int i = 0; i<16; i++)
            {
                command_list[i] = 0;
                command_params[i] = 0;
            }

            MissionWaypointMsg test_waypoint_1 = new MissionWaypointMsg( 0.0001f, 0.0f, 0.01f, 0.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 1, 10000, new MissionWaypointActionMsg(1,command_list, command_params));
            MissionWaypointMsg test_waypoint_2 = new MissionWaypointMsg(0.0002f, 0.0f, 0.01f, 0.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 1, 10000, new MissionWaypointActionMsg(1, command_list, command_params));
            Debug.Log(test_waypoint_1.ToYAMLString());

            MissionWaypointMsg[] test_waypoint_array = new MissionWaypointMsg[] { test_waypoint_1, test_waypoint_2 };
           /* for (int i = 0; i < 2; i++)
            {
                Debug.Log(test_waypoint_array[i].ToString());
            }*/
          
            

            MissionWaypointTaskMsg test_Task = new MissionWaypointTaskMsg( 5.0f , 0.0f, MissionWaypointTaskMsg.ActionOnFinish.RETURN_TO_HOME , 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.POINT , MissionWaypointTaskMsg.ActionOnRCLost.FREE , MissionWaypointTaskMsg.GimbalPitchMode.FREE , test_waypoint_array);

            /* for (int i = 0; i < 2; i++)
             {
                 MissionWaypointMsg[] test = test_Task.GetMissionWaypoints();
                 Debug.Log(test[i].ToString());
             }*/
            Debug.Log(test_Task.ToYAMLString());
            ros.CallService(service_name, string.Format("[{0}]", test_Task.ToYAMLString())); // try with and without []
        }

        if (Input.GetKeyUp("e"))
        {
            print("Execute test Waypoint Mission");
            string service_name = "/dji_sdk/mission_waypoint_action";
            ros.CallService(service_name, "[0]");
        }

        if (Input.GetKeyUp("i"))
        {
            print("get info for Waypoint Mission");
            string service_name = "/dji_sdk/mission_waypoint_getInfo";
            ros.CallService(service_name, "[0]");
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

