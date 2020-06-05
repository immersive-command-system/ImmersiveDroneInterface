using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;
using ISAACS;

public class ROSManager : MonoBehaviour {

    public enum DroneType { M100, M210, M600, Sprite };
    public enum DroneSubscribers { attitude, battery_state, flight_status, gimbal_angle, gps_health, gps_position, imu, rc, velocity, height_above_takeoff, local_position };

    public enum SensorType { PointCloud, Mesh, LAMP };
    public enum SensorSubscribers { surface_pointcloud, mesh, colorized_points_0, colorized_points_1, colorized_points_2, colorized_points_3, colorized_points_4, colorized_points_5 };

    [System.Serializable]
    public class ROSDroneConnectionInput
    {
        public string droneName;
        public string droneTag;
        public string ipAddress; // is this int32?
        public int port;
        public DroneType droneType;
        public List<DroneSubscribers> droneSubscribers;
    }

    [System.Serializable]
    public class ROSSensorConnectionInput
    {
        public string sensorName;
        public string sensorTag;
        public string ipAddress;
        public int port;
        public SensorType sensorType;
        public List<SensorSubscribers> sensorSubscribers;
    }

    public List<ROSDroneConnectionInput> DronesList;
    public List<ROSSensorConnectionInput> SensorsList;

    public bool success = false;
    public int uniqueID = 0;

    // Use this for initialization
    void Start ()
    {
        foreach ( ROSDroneConnectionInput rosDroneConnectionInput in DronesList)
        {
            InstantiateDrone(rosDroneConnectionInput);
        }

        foreach (ROSSensorConnectionInput rosSensorConnectionInput in SensorsList)
        {
            InstantiateSensor(rosSensorConnectionInput);
        }
    }

    private void InstantiateDrone(ROSDroneConnectionInput rosDroneConnectionInput)
    {
        DroneType droneType = rosDroneConnectionInput.droneType;
        string droneIP = rosDroneConnectionInput.ipAddress;
        int dronePort = rosDroneConnectionInput.port;
        List<string> droneSubscribers = new List<string>();

        foreach (DroneSubscribers subscriber in rosDroneConnectionInput.droneSubscribers)
        {
            droneSubscribers.Add(subscriber.ToString());
        }

        GameObject drone = new GameObject(rosDroneConnectionInput.droneName);
        drone.transform.parent = this.transform;
        drone.tag = rosDroneConnectionInput.droneTag;

        switch (droneType)
        {
            case DroneType.M100:
                Debug.Log("M100 created");
                M100_ROSDroneConnection M100_rosDroneConnection = drone.AddComponent<M100_ROSDroneConnection>();
                M100_rosDroneConnection.InitilizeDrone(uniqueID, droneIP, dronePort, droneSubscribers);
                break;

            case DroneType.M210:
                Debug.Log("M210 created");
                M210_ROSDroneConnection M210_rosDroneConnection = drone.AddComponent<M210_ROSDroneConnection>();
                M210_rosDroneConnection.InitilizeDrone(uniqueID, droneIP, dronePort, droneSubscribers);
                break;

            case DroneType.M600:
                Debug.Log("M600 created");
                M600_ROSDroneConnection M600_rosDroneConnection = drone.AddComponent<M600_ROSDroneConnection>();
                M600_rosDroneConnection.InitilizeDrone(uniqueID, droneIP, dronePort, droneSubscribers);
                break;

            case DroneType.Sprite:
                Debug.Log("Sprite class not implemented created");
                //Sprite_ROSDroneConnection drone_rosDroneConnection = drone.AddComponent<Sprite_ROSDroneConnection>();
                break;
            
            default:
                Debug.Log("No drone type selected");
                return;
        }

        // TODO: Uncomment after implementing ROSDroneConnection
        // drone.InitilizeDrone(uniqueID, droneIP, dronePort, droneSubscribers)
        uniqueID ++;
    }

    private void InstantiateSensor(ROSSensorConnectionInput rosSensorConnectionInput)
    {
        SensorType sensorType = rosSensorConnectionInput.sensorType;
        string sensorIP = rosSensorConnectionInput.ipAddress;
        int sensorPort = rosSensorConnectionInput.port;
        List<string> sensorSubscribers = new List<string>();

        foreach (SensorSubscribers subscriber in rosSensorConnectionInput.sensorSubscribers)
        {
            sensorSubscribers.Add(subscriber.ToString());
        }

        GameObject sensor = new GameObject(rosSensorConnectionInput.sensorName);
        sensor.transform.parent = this.transform;
        sensor.tag = rosSensorConnectionInput.sensorTag;

        switch (sensorType)
        {
            case SensorType.PointCloud:
                Debug.Log("PointCloud Sensor created");
                PointCloudSensor_ROSSensorConnection pcSensor_rosSensorConnection = sensor.AddComponent<PointCloudSensor_ROSSensorConnection>();
                pcSensor_rosSensorConnection.InitilizeSensor(uniqueID, sensorIP, sensorPort, sensorSubscribers);
                break;

            case SensorType.Mesh:
                Debug.Log("Mesh Sensor architecture not implemented yet");
                break;

            case SensorType.LAMP:
                Debug.Log("LAMP Sensor created");
                LampSensor_ROSSensorConnection lamp_rosSensorConnection = sensor.AddComponent<LampSensor_ROSSensorConnection>();
                lamp_rosSensorConnection.InitilizeSensor(uniqueID, sensorIP, sensorPort, sensorSubscribers);
                break;

            default:
                Debug.Log("No sensor type selected");
                return;
        }

        // TODO: Uncomment after implementing ROSDroneConnection
        // sensor.InitilizeSensor(uniqueID, sensorIP, sensorPort ,sensorSubscribers)
        uniqueID++;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
