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
    public bool connectionStatus = false;


    void Start()
    {
        // This is the IP of the linux computer that is connected to the drone. (make sure the ip starts with ws://[ip]) 
        ros = new ROSBridgeWebSocketConnection("ws://192.168.50.191", 9090);

        //ros.AddSubscriber(typeof(ObstacleSubscriber));
        //ros = new ROSBridgeWebSocketConnection("ws://192.168.50.48", 9090);
     
        //ros.AddSubscriber(typeof(EnvironmentSubscriber));
        //ros.AddSubscriber(typeof(DronePositionSubscriber));
        //ros.AddPublisher(typeof(UserpointPublisher));

        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));

        ros.AddSubscriber(typeof(M210_DronePositionSubscriber));
        ros.AddSubscriber(typeof(M210_Battery_Subscriber));
        ros.AddSubscriber(typeof(M210_GPSHealth_Subscriber));

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
        //float startLat = 37.91532757f;
       // float startLong = 122.33805556f;
       // Debug.Log("waypoint 1 unityX: " + WorldProperties.LatDiffMeters(startLat, 37.915701652f) / WorldProperties.Unity_X_To_Lat_Scale);
        //Debug.Log("waypoint 2 unityX: " + WorldProperties.LatDiffMeters(startLat, 37.915585270f) / WorldProperties.Unity_X_To_Lat_Scale);
        //Debug.Log("waypoint 3 unityX: " + WorldProperties.LatDiffMeters(startLat, 37.915457249f) / WorldProperties.Unity_X_To_Lat_Scale);

//        Debug.Log("waypoint 1 unityZ: " + WorldProperties.LongDiffMeters(startLong, 122.337967237f, startLat) / WorldProperties.Unity_Z_To_Long_Scale);
  //      Debug.Log("waypoint 2 unityZ: " + WorldProperties.LongDiffMeters(startLong, 122.338122805f, startLat) / WorldProperties.Unity_Z_To_Long_Scale);
    //    Debug.Log("waypoint 3 unityZ: " + WorldProperties.LongDiffMeters(startLong, 122.338015517f, startLat) / WorldProperties.Unity_Z_To_Long_Scale);


        ros.Render();

        if (Input.GetKeyUp("1"))
        {
            GetAuthority();
        }

        if (Input.GetKeyUp("2"))
        {
            GetVersion();
        }
        
        if (Input.GetKeyUp("3"))
        {
            Spin();
        }

        if (Input.GetKeyUp("4"))
        {
            StopSpinning();
        }

        if (Input.GetKeyUp("5"))
        {
            Takeoff();
        }

        if (Input.GetKeyUp("6"))
        {
            Land();
        }

        if (Input.GetKeyUp("q"))
        {

            uint[] command_list = new uint[16];
            uint[] command_params = new uint[16];
            for (int i = 0; i < 16; i++)
            {
                command_list[i] = 0;
                command_params[i] = 0;
            }

            MissionWaypointMsg test_waypoint_1 = new MissionWaypointMsg(37.915701652f, -122.337967237f, 20.0f, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
            MissionWaypointMsg test_waypoint_2 = new MissionWaypointMsg(37.915585270f, -122.338122805f, 20.0f, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
            MissionWaypointMsg test_waypoint_3 = new MissionWaypointMsg(37.915457249f, -122.338015517f, 20.0f, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));

            Debug.Log("Check float accuracy here" + test_waypoint_1.ToYAMLString());

            MissionWaypointMsg[] test_waypoint_array = new MissionWaypointMsg[] { test_waypoint_1, test_waypoint_2, test_waypoint_3 };
            /*for (int i = 0; i < 2; i++)
            {
                Debug.Log(test_waypoint_array[i].ToString());
            }
            */
            MissionWaypointTaskMsg test_Task = new MissionWaypointTaskMsg(15.0f, 15.0f, MissionWaypointTaskMsg.ActionOnFinish.RETURN_TO_HOME, 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.COORDINATED, MissionWaypointTaskMsg.ActionOnRCLost.FREE, MissionWaypointTaskMsg.GimbalPitchMode.FREE, test_waypoint_array);

            UploadMission(test_Task);
        }

        if (Input.GetKeyUp("w"))
        {
            CreateMission();
        }

        if (Input.GetKeyUp("e"))
        {
            ExecuteMission();
        }

        if (Input.GetKeyUp("i"))
        {
            InfoMission();
        }

        if (Input.GetKeyUp("["))
        {
            Debug.Log(WorldProperties.LongDiffMeters(122.2578f, 122.4783f, 37.8721f));
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


    public void CreateMission()
    {
        List<MissionWaypointMsg> missionMissionMsgList = new List<MissionWaypointMsg>();

        uint[] command_list = new uint[16];
        uint[] command_params = new uint[16];

        for (int i = 0; i < 16; i++)
        {
            command_list[i] = 0;
            command_params[i] = 0;
        }

        bool skip = true;

        foreach (Waypoint waypoint in WorldProperties.selectedDrone.waypoints)
        {
            if (skip)
            {
                skip = false;
                continue;
            }

            float x = waypoint.gameObjectPointer.transform.localPosition.x;
            float y = waypoint.gameObjectPointer.transform.localPosition.y;
            float z = waypoint.gameObjectPointer.transform.localPosition.z;
            
            
            Vector3 ROS_coordinates = new Vector3();
            Debug.Log(waypoint.id + " : " + waypoint.gameObjectPointer.transform.localPosition);

            //Debug.Log("Uploading waypoint at : " + ROS_coordinates);
            // Debug.Log("x coord: " + ROS_coordinates.x);
            // Debug.Log("y coord: " + ROS_coordinates.y);
            // Debug.Log("z coord: " + ROS_coordinates.z);

            // Peru's attempt at fixing the stuff as of 3/3/2020
            ROS_coordinates.x = WorldProperties.UnityXToLat(WorldProperties.droneHomeLat, x);
            ROS_coordinates.y = (y * WorldProperties.Unity_Y_To_Alt_Scale) - 1f;// + WorldProperties.droneHomeAlt; // The 100 has to be the same number that we divide the ROS coordinate by in M210_DronePositionSubscriber line 75
            ROS_coordinates.z = WorldProperties.UnityZToLong(WorldProperties.droneHomeLong, WorldProperties.droneHomeLat ,z);

            MissionWaypointMsg new_waypoint = new MissionWaypointMsg(ROS_coordinates.x, ROS_coordinates.z, ROS_coordinates.y, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
            Debug.Log("single waypoint info: " + new_waypoint);
            missionMissionMsgList.Add(new_waypoint);

           
        }

        MissionWaypointTaskMsg Task = new MissionWaypointTaskMsg(15.0f, 15.0f, MissionWaypointTaskMsg.ActionOnFinish.AUTO_LANDING, 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.POINT, MissionWaypointTaskMsg.ActionOnRCLost.FREE, MissionWaypointTaskMsg.GimbalPitchMode.FREE, missionMissionMsgList.ToArray());

        UploadMission(Task);
    }


    public void UploadMission(MissionWaypointTaskMsg Task)
    {
        print("Upload Waypoint Mission");
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

    public void PublishWaypointUpdateMessage(UserpointInstruction msg)
    {
        Debug.Log("ISAACS V1 Waypoint System Disabled.");
        //Debug.Log("Published new userpoint instruction: "+ msg.ToYAMLString());
        //ros.Publish(UserpointPublisher.GetMessageTopic(), msg);
    }

    public void SendServiceCall(string service, string args)
    {
        Debug.Log("Calling service: " + service);
        ros.CallService(service, args);
    }

}

