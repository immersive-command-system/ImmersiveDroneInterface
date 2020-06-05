using System;
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.sensor_msgs;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;

using ISAACS;


public class M100_ROSDroneConnection : MonoBehaviour, ROSTopicSubscriber
{
    // Drone state enums    
    public enum FlightStatusM100
    {
        ON_GROUND_STANDBY = 1,
        TAKEOFF = 2,
        IN_AIR_STANDBY = 3,
        LANDING = 4,
        FINISHING_LANDING = 5
    }
    public enum FlightStatus
    {
        STOPED = 0,
        ON_GROUND = 1,
        IN_AIR = 2
    }
    public enum CameraAction
    {
        SHOOT_PHOTO = 0,
        START_VIDEO = 1,
        STOP_VIDEO = 2
    }
    public enum DroneTask
    {
        GO_HOME = 1,
        TAKEOFF = 4,
        LAND = 6
    }
    public enum WaypointMissionAction
    {
        START = 0,
        STOP = 1,
        PAUSE = 2,
        RESUME = 3
    }
    
    // Private connection variables
    private ROSBridgeWebSocketConnection ros = null;
    string client_id;

    // Private state variables
    public bool sdk_ready
    {
        get
        {
            return ros != null;
        }
    }
    bool has_authority = false;
    BatteryStateMsg battery_state;
    FlightStatusM100 m100_flight_status;
    JoyMsg remote_controller_msg;
    Quaternion attitude = Quaternion.identity;
    Quaternion offset = Quaternion.Euler(90, 180, 0);
    IMUMsg imu;
    Vector3 velocity;
    float relative_altitude;        // Height above takeoff height.
    Vector3 local_position;
    Vector3 gimble_joint_angles;
    uint gps_health;
    NavSatFixMsg gps_position;

    // Initilize the drone
    public void InitilizeDrone(int uniqueID, string droneIP, int dronePort, List<string> droneSubscribers)
    {
        ros = new ROSBridgeWebSocketConnection("ws://" + droneIP, dronePort);
        client_id = uniqueID.ToString();

        foreach ( string subscriber in droneSubscribers)
        {
            ros.AddSubscriber("/dji_sdk/" + subscriber, this);
        }
    }

    // Update is called once per frame in Unity
    void Update()
    {
        if (ros != null)
        {
            ros.Render();
        }
    }

    // Common CallBack for all subscribers
    // TODO: Break down further if we ever need for the M100

    // ROS Topic Subscriber methods
    public ROSBridgeMsg OnReceiveMessage(string topic, JSONNode raw_msg, ROSBridgeMsg parsed = null)
    {
        ROSBridgeMsg result = null;
        // Writing all code in here for now. May need to move out to separate handler functions when it gets too unwieldy.
        switch (topic)
        {
            case "/dji_sdk/attitude":
                QuaternionMsg attitudeMsg = (parsed == null) ? new QuaternionMsg(raw_msg["quaternion"]) : (QuaternionMsg)parsed;
                attitude = offset * (new Quaternion(attitudeMsg.GetX(), attitudeMsg.GetY(), attitudeMsg.GetZ(), attitudeMsg.GetW()));
                result = attitudeMsg;
                break;
            case "/dji_sdk/battery_state":
                battery_state = (parsed == null) ? new BatteryStateMsg(raw_msg) : (BatteryStateMsg)parsed;
                result = battery_state;
                break;
            case "/dji_sdk/flight_status":
                m100_flight_status = (FlightStatusM100)(new UInt8Msg(raw_msg)).GetData();
                break;
            case "/dji_sdk/gimbal_angle":
                Vector3Msg gimbleAngleMsg = (parsed == null) ? new Vector3Msg(raw_msg["vector"]) : (Vector3Msg)parsed;
                gimble_joint_angles = new Vector3((float)gimbleAngleMsg.GetX(), (float)gimbleAngleMsg.GetY(), (float)gimbleAngleMsg.GetZ());
                result = gimbleAngleMsg;
                break;
            case "/dji_sdk/gps_health":
                gps_health = (parsed == null) ? (new UInt8Msg(raw_msg)).GetData() : ((UInt8Msg)parsed).GetData();
                break;
            case "/dji_sdk/gps_position":
                gps_position = (parsed == null) ? new NavSatFixMsg(raw_msg) : (NavSatFixMsg)parsed;
                result = gps_position;
                break;
            case "/dji_sdk/imu":
                imu = (parsed == null) ? new IMUMsg(raw_msg) : (IMUMsg)parsed;
                result = imu;
                break;
            case "/dji_sdk/rc":
                remote_controller_msg = (parsed == null) ? new JoyMsg(raw_msg) : (JoyMsg)parsed;
                result = remote_controller_msg;
                break;
            case "/dji_sdk/velocity":
                Vector3Msg velocityMsg = (parsed == null) ? new Vector3Msg(raw_msg["vector"]) : (Vector3Msg)parsed;
                velocity = new Vector3((float)velocityMsg.GetX(), (float)velocityMsg.GetY(), (float)velocityMsg.GetZ());
                result = velocityMsg;
                break;
            case "/dji_sdk/height_above_takeoff":
                relative_altitude = (parsed == null) ? (new Float32Msg(raw_msg)).GetData() : ((Float32Msg)parsed).GetData();
                break;
            case "/dji_sdk/local_position":
                PointMsg pointMsg = (parsed == null) ? new PointMsg(raw_msg["point"]) : (PointMsg)parsed;
                local_position = new Vector3(pointMsg.GetX(), pointMsg.GetY(), pointMsg.GetZ());
                result = pointMsg;
                Debug.Log(result);
                break;
            default:
                Debug.LogError("Topic not implemented: " + topic);
                break;
        }
        return result;
    }
    public string GetMessageType(string topic)
    {
        switch (topic)
        {
            case "/dji_sdk/attitude":
                return "geometry_msgs/QuaternionStamped";
            case "/dji_sdk/battery_state":
                return "sensor_msgs/BatteryState";
            case "/dji_sdk/flight_status":
                return "std_msgs/UInt8";
            case "/dji_sdk/gimbal_angle":
                return "geometry_msgs/Vector3Stamped";
            case "/dji_sdk/gps_health":
                return "std_msgs/UInt8";
            case "/dji_sdk/gps_position":
                return "sensor_msgs/NavSatFix";
            case "/dji_sdk/imu":
                return "sensor_msgs/Imu";
            case "/dji_sdk/rc":
                return "sensor_msgs/Joy";
            case "/dji_sdk/velocity":
                return "geometry_msgs/Vector3Stamped";
            case "/dji_sdk/height_above_takeoff":
                return "std_msgs/Float32";
            case "/dji_sdk/local_position":
                return "geometry_msgs/PointStamped";
        }
        Debug.LogError("Topic " + topic + " not registered.");
        return "";
    }

    // Service Calls and corresponding callbacks
    // TODO: Change Debug to return

    public void FetchDroneVersion()
    {
        string service_name = "dji_sdk/query_drone_version";
        ros.CallService(HandleDroneVersionResponse, service_name, string.Format("{0} {1}", client_id, service_name));
    }
    void HandleDroneVersionResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Drone: {0} (Version {1})", response["hardware"].Value, response["version"].AsInt);
    }

    public void ActivateDrone()
    {
        string service_name = "/dji_sdk/activation";
        ros.CallService(HandleActivationResponse, service_name, string.Format("{0} {1}", client_id, service_name));
    }
    void HandleActivationResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Activation {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
    }

    public void SetSDKControl(bool control)
    {
        string service_name = "/dji_sdk/sdk_control_authority";
        ros.CallService(HandleSetSDKControlResponse, service_name, string.Format("{0} {1}", client_id, service_name), string.Format("[{0}]", (control ? 1 : 0)));
        has_authority = control;
    }
    void HandleSetSDKControlResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log(response.ToString());
        Debug.LogFormat("Control request {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
        //if (response["result"].AsBool == true)
        //{
        //    has_authority = requested_authority;
        //}
    }

    public void ChangeArmStatusTo(bool armed)
    {
        string service_name = "/dji_sdk/drone_arm_control";
        ros.CallService(HandleArmResponse, service_name, string.Format("{0} {1}", client_id, service_name), string.Format("[{0}]", (armed ? 1 : 0)));
    }
    void HandleArmResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Arm/Disarm request {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
    }

    public void ExecuteTask(DroneTask task)
    {
        string service_name = "/dji_sdk/drone_task_control";
        ros.CallService(HandleTaskResponse, service_name, string.Format("{0} {1}", client_id, service_name), string.Format("[{0}]", (int)task));
    }
    void HandleTaskResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Task request {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
    }

    public void SetLocalPosOriginToCurrentLocation()
    {
        string service_name = "/dji_sdk/set_local_pos_ref";
        ros.CallService(HandleSetLocalPosOriginResponse, service_name, string.Format("{0} {1}", client_id, service_name));
    }
    void HandleSetLocalPosOriginResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Local position origin set {0}", (response["result"].AsBool ? "succeeded" : "failed"));
    }

    public void ExecuteCameraAction(CameraAction action)
    {
        string service_name = "/dji_sdk/camera_action";
        ros.CallService(HandleCameraActionResponse, service_name, string.Format("{0} {1}", client_id, service_name), args: string.Format("[{0}]", (int)action));
    }
    void HandleCameraActionResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Camera action {0}", (response["result"].AsBool ? "succeeded" : "failed"));
    }

    public void FetchMissionStatus()
    {
        string service_name = "/dji_sdk/mission_status";
        ros.CallService(HandleMissionStatusResponse, service_name, string.Format("{0} {1}", client_id, service_name));
    }
    void HandleMissionStatusResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Waypoint Count: {0}\nHotpoint Count: {1}", response["waypoint_mission_count"], response["hotpoint_mission_count"]);
    }

    public void UploadWaypointsTask(MissionWaypointTaskMsg task)
    {
        string service_name = "/dji_sdk/mission_waypoint_upload";
        ros.CallService(HandleUploadWaypointsTaskResponse, service_name, string.Format("{0} {1}", client_id, service_name), args: string.Format("[{0}]", task.ToYAMLString()));
    }
    void HandleUploadWaypointsTaskResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Waypoint task upload {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
    }

    public void SendWaypointAction(WaypointMissionAction action)
    {
        string service_name = "/dji_sdk/mission_waypoint_action";
        ros.CallService(HandleWaypointActionResponse, service_name, string.Format("{0} {1}", client_id, service_name), args: string.Format("[{0}]", action));
    }
    void HandleWaypointActionResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Waypoint action {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
    }

    public void FetchCurrentWaypointMission()
    {
        string service_name = "/dji_sdk/mission_waypoint_getInfo";
        ros.CallService(HandleCurrentWaypointMissionResponse, service_name, string.Format("{0} {1}", client_id, service_name));
    }
    void HandleCurrentWaypointMissionResponse(JSONNode response)
    {
        MissionWaypointTaskMsg waypoint_task = new MissionWaypointTaskMsg(response["values"]);
        Debug.LogFormat("Current waypoint mission: \n{0}", waypoint_task.ToYAMLString());
    }

    public void FetchWaypointSpeed()
    {
        string service_name = "/dji_sdk/mission_waypoint_getSpeed";
        ros.CallService(HandleWaypointSpeedResponse, service_name, string.Format("{0} {1}", client_id, service_name));
    }
    void HandleWaypointSpeedResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Current waypoint speed: {0}", response["speed"].AsFloat);
    }

    public void SetWaypointSpeed(float speed)
    {
        string service_name = "/dji_sdk/mission_waypoint_setSpeed";
        ros.CallService(HandleSetWaypointSpeedResponse, service_name, string.Format("{0} {1}", client_id, service_name), args: string.Format("[{0}]", speed));
    }
    void HandleSetWaypointSpeedResponse(JSONNode response)
    {
        response = response["values"];
        Debug.LogFormat("Set waypoint speed {0} (ACK: {1})", (response["result"].AsBool ? "succeeded" : "failed"), response["ack_data"].AsInt);
    }

    public void Subscribe240p(bool front_right, bool front_left, bool down_front, bool down_back)
    {
        string serviceName = "/dji_sdk/stereo_240p_subscription";
        string id = string.Format("{0} {1} subscribe", client_id, serviceName);
        string args = string.Format("[{0} {1} {2} {3} 0]", front_right ? 1 : 0, front_left ? 1 : 0, down_front ? 1 : 0, down_back ? 1 : 0);
        ros.CallService(HandleSubscribe240pResponse, serviceName, id, args);
    }
    void HandleSubscribe240pResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Subscribe to 240p feeds " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void Unsubscribe240p()
    {
        string serviceName = "/dji_sdk/stereo_240p_subscription";
        string id = string.Format("{0} {1} unsubscribe", client_id, serviceName);
        ros.CallService(HandleUnsubscribe240pResponse, serviceName, id, "[0 0 0 0 1]");
    }
    void HandleUnsubscribe240pResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Unsubscribe to 240p feeds " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void SubscribeDepthFront()
    {
        string serviceName = "/dji_sdk/stereo_depth_subscription";
        string id = string.Format("{0} {1} subscribe", client_id, serviceName);
        ros.CallService(HandleSubscribeDepthFrontResponse, serviceName, id, "[1 0]");
    }
    void HandleSubscribeDepthFrontResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Subscribe front depth feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void UnsubscribeDepthFront()
    {
        string serviceName = "/dji_sdk/stereo_depth_subscription";
        string id = string.Format("{0} {1} unsubscribe", client_id, serviceName);
        ros.CallService(HandleUnsubscribeDepthFrontResponse, serviceName, id, "[0 1]");
    }
    void HandleUnsubscribeDepthFrontResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Unsubscribe front depth feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void SubscribeVGAFront(bool use_20Hz)
    {
        string serviceName = "/dji_sdk/stereo_vga_subscription";
        string id = string.Format("{0} {1} subscribe", client_id, serviceName);
        ros.CallService(HandleSubscribeVGAFrontResponse, serviceName, id, string.Format("[{0} 1 0]", use_20Hz ? 0 : 1));
    }
    void HandleSubscribeVGAFrontResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Subscribe VGA front feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void UnsubscribeVGAFront()
    {
        string serviceName = "/dji_sdk/stereo_vga_subscription";
        string id = string.Format("{0} {1} unsubscribe", client_id, serviceName);
        ros.CallService(HandleUnsubscribeVGAFrontResponse, serviceName, id, "[0 0 1]");
    }
    void HandleUnsubscribeVGAFrontResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Unsubscribe VGA front feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void SubscribeFPV()
    {
        string serviceName = "/dji_sdk/setup_camera_stream";
        string id = string.Format("{0} {1} subscribe FPV", client_id, serviceName);
        ros.CallService(HandleSubscribeFPVResponse, serviceName, id, "[0 1]");
    }
    void HandleSubscribeFPVResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Subscribe FPV feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void UnsubscribeFPV()
    {
        string serviceName = "/dji_sdk/setup_camera_stream";
        string id = string.Format("{0} {1} unsubscribe FPV", client_id, serviceName);
        ros.CallService(HandleUnsubscribeFPVResponse, serviceName, id, "[0 0]");
    }
    void HandleUnsubscribeFPVResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Unsubscribe FPV feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void SubscribeMainCamera()
    {
        string serviceName = "/dji_sdk/setup_camera_stream";
        string id = string.Format("{0} {1} subscribe MainCamera", client_id, serviceName);
        ros.CallService(HandleSubscribeMainCameraResponse, serviceName, id, "[1 1]");
    }
    void HandleSubscribeMainCameraResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Subscribe MainCamera feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    public void UnsubscribeMainCamera()
    {
        string serviceName = "/dji_sdk/setup_camera_stream";
        string id = string.Format("{0} {1} unsubscribe MainCamera", client_id, serviceName);
        ros.CallService(HandleUnsubscribeMainCameraResponse, serviceName, id, "[1 0]");
    }
    void HandleUnsubscribeMainCameraResponse(JSONNode response)
    {
        response = response["values"];
        Debug.Log("Unsubscribe MainCamera feed " + ((response["result"].AsBool) ? "succeeded" : "failed"));
    }

    // Query Drone State Variables

    public bool HasAuthority()
    {
        return has_authority;
    }

    public FlightStatusM100 GetFlightStatusM100()
    {
        return m100_flight_status;
    }

    public Quaternion GetAttitude()
    {
        return attitude;
    }

    public NavSatFixMsg GetGPSPosition()
    {
        return gps_position;
    }

    public float GetHeightAboveTakeoff()
    {
        return relative_altitude;
    }

    public Vector3 GetLocalPosition()
    {
        return local_position;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public Vector3 GetGimbleJointAngles()
    {
        return gimble_joint_angles;
    }

    public float GetGPSHealth()
    {
        return gps_health;
    }

    // Helper Methods
    private void subscribe_to_all_sdk_topics(ROSBridgeWebSocketConnection connection)
    {
        connection.AddSubscriber("/dji_sdk/attitude", this);
        connection.AddSubscriber("/dji_sdk/battery_state", this);
        connection.AddSubscriber("/dji_sdk/flight_status", this);
        connection.AddSubscriber("/dji_sdk/gimbal_angle", this);
        connection.AddSubscriber("/dji_sdk/gps_health", this);
        connection.AddSubscriber("/dji_sdk/gps_position", this);
        connection.AddSubscriber("/dji_sdk/imu", this);
        connection.AddSubscriber("/dji_sdk/rc", this);
        connection.AddSubscriber("/dji_sdk/velocity", this);
        connection.AddSubscriber("/dji_sdk/height_above_takeoff", this);
        connection.AddSubscriber("/dji_sdk/local_position", this);
    }

    // Not sure what they do

    DateTime epoch = new DateTime(1970, 1, 1);
    int[] dummy = new int[0];

    int seq_no_point = 0;
    public void PublishRelativeSetPoint(Vector2 position_offset, float relative_altitude, float yaw_angle)
    {
        int num_seconds = DateTime.UtcNow.Second - epoch.Second;
        HeaderMsg header = new HeaderMsg(seq_no_point++, new TimeMsg(num_seconds, (int)(DateTime.UtcNow.Ticks - ((long)num_seconds) * (10000000))), "");

        float[] axes = new float[4];
        axes[0] = position_offset.x;
        axes[1] = position_offset.y;
        axes[2] = relative_altitude;
        axes[3] = yaw_angle;

        JoyMsg msg = new JoyMsg(header, axes, dummy);
        ros.Publish("/dji_sdk/flight_control_setpoint_ENUposition_yaw", msg);
    }

    int seq_no_rate = 0;
    public void PublishRateSetpoint(float roll, float pitch, float height, float yaw_rate)
    {
        int num_seconds = DateTime.UtcNow.Second - epoch.Second;
        HeaderMsg header = new HeaderMsg(seq_no_rate++, new TimeMsg(num_seconds, (int)(DateTime.UtcNow.Ticks - ((long)num_seconds) * (10000000))), "");

        float[] axes = new float[4];
        axes[0] = Mathf.Deg2Rad * roll;
        axes[1] = Mathf.Deg2Rad * pitch;
        axes[2] = height;
        axes[3] = yaw_rate;

        JoyMsg msg = new JoyMsg(header, axes, dummy);
        ros.Publish("/dji_sdk/flight_control_setpoint_rollpitch_yawrate_zposition", msg);
    }

    private void publish_to_all_sdk_topics(ROSBridgeWebSocketConnection connection)
    {
        connection.AddPublisher("/dji_sdk/flight_control_setpoint_ENUposition_yaw", "sensor_msgs/Joy");
        connection.AddPublisher("/dji_sdk/flight_control_setpoint_rollpitch_yawrate_zposition", "sensor_msgs/Joy");
    }

    const float EARTH_RADIUS_METERS = 6378100;
    public static Vector2 VectorFromGPS(NavSatFixMsg position1, NavSatFixMsg position2)
    {
        double lon_diff_rad = Mathf.Deg2Rad * (position2.GetLongitude() - position1.GetLongitude());
        double a = Math.Pow(Math.Sin(Mathf.Deg2Rad * (position2.GetLatitude() - position1.GetLatitude()) * 0.5), 2) +
            Math.Cos(Mathf.Deg2Rad * position1.GetLatitude()) * Math.Cos(Mathf.Deg2Rad * position2.GetLatitude()) *
            Math.Pow(Math.Sin(lon_diff_rad * 0.5), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EARTH_RADIUS_METERS * c;

        double x = Math.Cos(Mathf.Deg2Rad * position1.GetLatitude()) * Math.Sin(Mathf.Deg2Rad * position2.GetLatitude()) -
            Math.Sin(Mathf.Deg2Rad * position1.GetLatitude()) * Math.Cos(Mathf.Deg2Rad * position2.GetLatitude()) *
            Math.Cos(lon_diff_rad);
        double y = Math.Cos(Mathf.Deg2Rad * position2.GetLatitude()) * Math.Sin(lon_diff_rad);
        double bearing_angle = Math.Atan2(y, x);
        Vector2 bearing = new Vector2((float)Math.Sin(bearing_angle), (float)Math.Cos(bearing_angle));

        return bearing * (float)distance;
    }

}
