using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;
using ISAACS;


public class DebuggingManager : MonoBehaviour {

    [Header("Radiation Buttons")]
    public string subscribeSurfacePointCloud = "6";
    public string subscribeColorized0 = "0";
    public string subscribeColorized1 = "1";
    public string subscribeColorized2 = "2";
    public string subscribeColorized3 = "3";
    public string subscribeColorized4 = "4";
    public string subscribeColorized5 = "5";

    [Header("Drone Commands")]
    public string getAuthority = "q";
    public string getVersion = "w";
    public string takeoffDrone= "e";
    public string landDrone = "r";
    public string uploadTestMission = "t";
    public string missionInfo = "y";
    public string executeUploadedMission = "u";

    [Header("Application Variables")]
    public WorldProperties worldProperties;
    public ROSDroneConnection rosDroneConnection;
    //public ROSSensorConnection rosSensorConnection;

    // Use this for initialization
    void Start () {
        worldProperties = GameObject.FindGameObjectWithTag("World").GetComponent<WorldProperties>();
        rosDroneConnection = GameObject.FindGameObjectWithTag("World").GetComponent<ROSDroneConnection>();
        //rosSensorConnection = GameObject.FindGameObjectWithTag("World").GetComponent<ROSSensorConnection>();
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyUp(subscribeColorized0))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_Colorized_0();
        }

        if (Input.GetKeyUp(subscribeColorized1))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_Colorized_1();
        }

        if (Input.GetKeyUp(subscribeColorized2))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_Colorized_2();
        }

        if (Input.GetKeyUp(subscribeColorized3))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_Colorized_3();
        }

        if (Input.GetKeyUp(subscribeColorized4))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_Colorized_4();
        }

        if (Input.GetKeyUp(subscribeColorized5))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_Colorized_5();
        }

        if (Input.GetKeyUp(subscribeSurfacePointCloud))
        {
            // TODO: Switch to rosSensorConnection
            rosDroneConnection.LampSubscribe_SurfacePointcloud();
        }
        
        if (Input.GetKeyUp(getAuthority))
        {
            rosDroneConnection.GetAuthority();
        }

        if (Input.GetKeyUp(getVersion))
        {
            rosDroneConnection.GetVersion();
        }

        if (Input.GetKeyUp(takeoffDrone))
        {
            rosDroneConnection.Takeoff();
        }

        if (Input.GetKeyUp(landDrone))
        {
            rosDroneConnection.Land();
        }

        if (Input.GetKeyUp(uploadTestMission))
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

            MissionWaypointTaskMsg test_Task = new MissionWaypointTaskMsg(15.0f, 15.0f, MissionWaypointTaskMsg.ActionOnFinish.RETURN_TO_HOME, 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.COORDINATED, MissionWaypointTaskMsg.ActionOnRCLost.FREE, MissionWaypointTaskMsg.GimbalPitchMode.FREE, test_waypoint_array);

            rosDroneConnection.UploadMission(test_Task);
        }

        if (Input.GetKeyUp(missionInfo))
        {
            rosDroneConnection.InfoMission();
        }

        if (Input.GetKeyUp(executeUploadedMission))
        {
            rosDroneConnection.ExecuteMission();
        }

    }
}
