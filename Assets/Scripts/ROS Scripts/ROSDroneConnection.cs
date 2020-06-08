using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;
using ISAACS;

/// <summary>
/// This class uses ROSBridgeWebSocketConnection to communicate between Unity and on-board computer.
/// It first creates a
/// ROSBridgeWebSocketConnection with the onboard computer for all Unity <--> Manifold communication.
/// It allows to call ROS Services to control the drone (takeoff, create waypoint mission, execute mission, etc).
/// Through simple keyboard inputs, these ROS Service calls can be tested.
/// </summary>
public class ROSDroneConnection : MonoBehaviour
{
    private ROSBridgeWebSocketConnection ros = null;

    // TODO: Remove 
    //***
    private ROSBridgeWebSocketConnection lamp_ros_variable = null;
    private ROSBridgeWebSocketConnection lamp_ros_constant = null;
    // ***

    [Header("Drone Variables")]
    public string ManifoldIP = "192.168.60.191";
    public bool connectionStatus = false;

    [Header("TO REMOVE: Sensor Variables")]
    public bool implementLamp = true;
    public string LampIP = "192.168.1.73";
    public int pointCloudLevel;

    [Header("Drone Subscribers")]
    public bool DronePosition = true;
    public bool DroneBattery = false;
    public bool GPSHealth = false;


    void Start()
    {
        // This is the IP of the on-board linux computer (Manifold). (make sure the ip starts with ws://[ip])
        ros = new ROSBridgeWebSocketConnection("ws://" + ManifoldIP, 9090);

        // Create ROS Subscribe that listen to relevant ROS topics. Using these ROS subscribers, the Unity projects 
        // gains access to important drone information such as drone real-time GPS position, battery life,
        // GPS signal quality, Radiation Point Cloud etc.
        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));

        if (DronePosition)
        {
            ros.AddSubscriber(typeof(M210_DronePositionSubscriber));
        }

        if (DroneBattery)
        {
            ros.AddSubscriber(typeof(M210_Battery_Subscriber));
        }

        if (GPSHealth)
        {
            ros.AddSubscriber(typeof(M210_GPSHealth_Subscriber));
        }

        // TODO: Remove
        // ***

        // This is the IP of the LAMP data computer. (make sure the ip starts with ws://[ip])

        if (implementLamp)
        {

            lamp_ros_constant = new ROSBridgeWebSocketConnection("ws://" + LampIP, 9090);
            lamp_ros_variable = new ROSBridgeWebSocketConnection("ws://" + LampIP, 9090);

            lamp_ros_variable.AddSubscriber(typeof(ColorizedCloud4Subscriber));
            pointCloudLevel = 4;
        
            lamp_ros_constant.Connect();
            lamp_ros_variable.Connect();
        }

        // ***

        Debug.Log("Sending connection attempt to ROS");
        ros.Connect();
        connectionStatus = true;

    }

    /// <summary>
    /// Extremely important to disconnect from ROS. OTherwise packets continue to flow
    /// </summary>
    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting from ROS");
        if (ros != null)
        {
            ros.Disconnect();
        }

        //TODO: Remove
        // ***

        if (lamp_ros_constant != null)
        {
            lamp_ros_constant.Disconnect();
        }
        
        if (lamp_ros_variable != null)
        {
            lamp_ros_variable.Disconnect();
        }

        // ***

    }

    /// <summary>
    /// Update is called once per frame in Unity. Here, we take care of keyboard inputs for when the Unity project is running.
    /// Each key press results in the corresponding method to be called.
    /// Method descriptions can be found further below.
    /// </summary>
    void Update()
    {
        ros.Render();

        //TODO: Remove
        // ***

        if (implementLamp)
        {
            lamp_ros_constant.Render();
            lamp_ros_variable.Render();
        }

        //TODO: Remove
        // ***

    }

    //TODO: Remove
    // ***


    /// <summary>
    /// Helper functions for subscribing to different LAMP topics.
    /// </summary>
    /// <returns></returns>

    private void resetLampConnection(System.Type subscriber)
    {
        if (lamp_ros_variable != null)
        {
            lamp_ros_variable.Disconnect();
        }

        //lamp_ros_variable = new ROSBridgeWebSocketConnection("ws://"+LampIP, 9090);
        lamp_ros_variable.AddSubscriber(subscriber);
        lamp_ros_variable.Connect();
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
    
    // ***


    // Functions for ROS begin here.
    public bool GetConnectionStatus()
    {
        return connectionStatus;
    }

    /// <summary>
    /// Running this service gives the onboard computer (Manifold 2 in our case) control over the drone.
    /// Attempts to control the drone through the Manifold before calling this ROS Service will not work.
    /// </summary>
    public void GetAuthority()
    {
        print("Get Authority");
        string service_name = "/dji_sdk/sdk_control_authority";
        ros.CallService(service_name, "[1]");
    }
    /// <summary> Returns the version of the drone. </summary>
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

    /// <summary> Calls the ROS service to make the rotors start spinning (the drone won't move, it's rotors will just begin to spin). </summary>
    public void Spin()
    {
        print("Spin");
        string service_name = "/dji_sdk/drone_arm_control";
        ros.CallService(service_name, "[1]");
    }
    /// <summary> Calls the ROS Service to stop the rotors from spinning. </summary>
    public void StopSpinning()
    {
        print("Stop spinning");
        string service_name = "/dji_sdk/drone_arm_control";
        ros.CallService(service_name, "[0]");
    }
    /// <summary> Calls the ROS Service that makes the drone take off and hover in place just above the ground. </summary>
    public void Takeoff()
    {
        print("Takeoff");
        string service_name = "/dji_sdk/drone_task_control";
        ros.CallService(service_name, "[4]");
    }
    /// <summary> Makes the drone land in place. (Different from returning to home).</summary>
    public void Land()
    {
        print("Land");
        string service_name = "/dji_sdk/drone_task_control";
        ros.CallService(service_name, "[6]");
    }

    /// <summary>
    /// Non-dynamic waypoint system. Tested and works will be depreciated when dynamic waypoint system works.
    /// Creates a mission task from the user-created waypoints stored in WorldProperties and uploads it to the drone.
    /// The drone will not start flying. It will store the mission, and wait for an excecute mission call before flying.
    /// </summary>
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

            double ROS_x = WorldProperties.UnityXToLat(WorldProperties.droneHomeLat, x);
            float ROS_y = (y * WorldProperties.Unity_Y_To_Alt_Scale) - 1f;
            double ROS_z = WorldProperties.UnityZToLong(WorldProperties.droneHomeLong, WorldProperties.droneHomeLat, z);

            MissionWaypointMsg new_waypoint = new MissionWaypointMsg(ROS_x, ROS_z, ROS_y, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
            Debug.Log("single waypoint info: " + new_waypoint);
            missionMissionMsgList.Add(new_waypoint);
        }
        MissionWaypointTaskMsg Task = new MissionWaypointTaskMsg(15.0f, 15.0f, MissionWaypointTaskMsg.ActionOnFinish.AUTO_LANDING, 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.POINT, MissionWaypointTaskMsg.ActionOnRCLost.FREE, MissionWaypointTaskMsg.GimbalPitchMode.FREE, missionMissionMsgList.ToArray());
        UploadMission(Task);
    }

    /// <summary>
    /// Uploads the given Waypoint mission task to the drone.
    /// The drone will not start flying. It will store the mission, and wait for an excecute_mission call before actually flying.
    /// </summary>
    public void UploadMission(MissionWaypointTaskMsg Task)
    {
        print("Upload Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_upload";
        Debug.Log(Task.ToYAMLString());
        ros.CallService(service_name, string.Format("[{0}]", Task.ToYAMLString())); // try with and without []
    }

    /// <summary> Executes whatever mission was previously uploaded. (This makes the drone actually fly) </summary>
    public void ExecuteMission()
    {
        print("Execute test Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_action";
        ros.CallService(service_name, "[0]");
    }

    /// <summary>
    /// Pauses the mission currently in flight
    /// </summary>
    public void PauseMission()
    {
        print("Pause Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_action";
        ros.CallService(service_name, "[2]");
    }

    /// <summary>
    /// Resumes the mission currently in flight
    /// </summary>
    public void ResumeMission()
    {
        print("Resume Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_action";
        ros.CallService(service_name, "[3]");
    }

    /// <summary>
    /// Returns information regarding the currently uploaded mission. This will print all the waypoints left in the mission.
    /// </summary>
    public void InfoMission()
    {
        print("get info for Waypoint Mission");
        string service_name = "/dji_sdk/mission_waypoint_getInfo";
        ros.CallService(service_name, "[0]");
    }
    /// <summary>
    /// Makes the drone fly to its home point.
    /// It first flies laterally until its above the home coordinate, and then lands.
    /// </summary>
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
    /// <summary> a general helper method to call any ROS Service with any argument.</summary>
    public void SendServiceCall(string service, string args)
    {
        Debug.Log("Calling service: " + service);
        ros.CallService(service, args);
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
