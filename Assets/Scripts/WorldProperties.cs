namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using ROSBridgeLib;
    using ROSBridgeLib.std_msgs;
    using ROSBridgeLib.interface_msgs;

    /// <summary>
    /// This is the only class that should have static variables or functions that are consistent throughout the entire program.
    /// </summary>
    public class WorldProperties : MonoBehaviour
    {
        public static double planningTime;
        public static double runtime;
        public GameObject droneBaseObject;
        public GameObject waypointBaseObject;
        public GameObject torus;

        public static Shader clipShader;

        public static Dictionary<char, Drone> dronesDict;
        public static Dictionary<char, GameObject> hoopsDict;

        public static Drone selectedDrone;

        public static GameObject worldObject;
        public static GameObject placementPlane;

        public static Vector3 actualScale;
        public static Vector3 currentScale;

        public static Vector3 droneModelOffset;
        public static Vector3 torusModelOffset;

        private static float maxHeight;
        public static char nextDroneId;


        public static List<GameObject> obstacles;
        public static TextAsset asset; //used in ROSDroneSubscriber
        public static GameObject closestObstacle;
        public static float closestDist; //between obstacle and drone
        public static HashSet<int> obstacleids; //used in ObstacleSubscriber
        public static List<string> obstacleDistsToPrint;

        // M210 ROs-Unity conversion variables
        public static float earth_radius = 6378137;
        public static Vector3 initial_DroneROS_Position = Vector3.zero;
        public static Vector3 initial_DroneUnity_Position = Vector3.zero;
        public static float ROS_to_Unity_Scale = 0.0f;

        // Use this for initialization
        void Start()
        {
            planningTime = 0;
            runtime = 0;

            selectedDrone = null;
            dronesDict = new Dictionary<char, Drone>(); // Collection of all the drone classObjects
            hoopsDict = new Dictionary<char, GameObject>(); // Collection of all the hoop gameObjects
            nextDroneId = 'A'; // Used as an incrementing key for the dronesDict and for a piece of the communication about waypoints across the ROSBridge
            worldObject = gameObject;
            placementPlane = GameObject.FindWithTag("Ground");
            actualScale = new Vector3(1, 1, 1);
            currentScale = new Vector3(1, 1, 1);

            droneModelOffset = new Vector3(0.0044f, -0.0388f, 0.0146f);
            torusModelOffset = new Vector3(-.1f, -.07f, 0f);

            maxHeight = 5;
            clipShader = GameObject.FindWithTag("Ground").GetComponent<Renderer>().material.shader;

            obstacles = new List<GameObject>();
            asset = (TextAsset)Resources.Load("test");
            closestObstacle = null;
            closestDist = -1;
            obstacleids = new HashSet<int>();
            obstacleDistsToPrint = new List<string>();

            NewDrone();
        }

       
        private void Update()
        {
            planningTime += Time.deltaTime;
            // float relative_scale =
        }

        /// <summary>
        /// Returns the maximum height that a waypoint can be placed at
        /// </summary>
        /// <returns></returns>
        public static float GetMaxHeight()
        {
            return (maxHeight * (actualScale.y)) + worldObject.transform.position.y;
        }

        /// <summary>
        /// Recursively adds the clipShader to the parent and all its children
        /// </summary>
        /// <param name="parent">The topmost container of the objects which will have the shader added to them</param>
        public static void AddClipShader(Transform parent)
        {
            if (parent.GetComponent<Renderer>())
            {
                parent.GetComponent<Renderer>().material.shader = clipShader;
            }

            foreach (Transform child in parent)
            {
                AddClipShader(child);
            }
        }

        /// <summary>
        /// Creates a new drone
        /// </summary>
        public static void NewDrone()
        {
            if (!GameObject.FindWithTag("Drone"))
            {
                Drone newDrone = new Drone(worldObject.transform.position + new Vector3(0, 0.1f, 0));
                selectedDrone = newDrone;
            }
        }


        public static void M210_FirstPositionCallback(M210_DronePositionMsg new_ROSPosition, Vector3 _initial_DroneUnity_Position)
        {
            float lat_rad = Mathf.PI * new_ROSPosition._lat / 180;
            float alt_rad = Mathf.PI * new_ROSPosition._altitude / 180;
            float long_rad = Mathf.PI * new_ROSPosition._long / 180;


            float x_pos = (earth_radius + alt_rad) * (float)Math.Cos(lat_rad) * (float)Math.Cos(long_rad);
            float y_pos = (earth_radius + alt_rad) * (float)Math.Cos(lat_rad) * (float)Math.Sin(long_rad);
            float z_pos = (earth_radius + alt_rad) * (float)Math.Sin(lat_rad);



            initial_DroneROS_Position.x = x_pos;
            initial_DroneROS_Position.y = y_pos;
            initial_DroneROS_Position.z = z_pos;

            initial_DroneUnity_Position = _initial_DroneUnity_Position;

            ROS_to_Unity_Scale = initial_DroneROS_Position.magnitude / initial_DroneUnity_Position.magnitude;


            Debug.Log("Initial Drone ROS Pos: " + initial_DroneROS_Position);
            Debug.Log("Initial Drone Unity Pos: " + initial_DroneUnity_Position);
            Debug.Log("Scale set to: " + ROS_to_Unity_Scale);
        }


        /// <summary>
        /// ROS (Spherical) to Unity (Cartesian) Coordinate conversion for the M210
        /// Assumed the earth is a sphere to greatly simplify the math
        /// </summary>
        /// <param name="ROS_lat"></param>
        /// <param name="ROS_alt"></param>
        /// <param name="ROS_long"></param>
        /// <returns></returns>
        public static Vector3 M210_ROSToUnity(float ROS_lat, float ROS_alt, float ROS_long)
        {
            /*float lat_rad = Mathf.PI * ROS_lat / 180;
            float alt_rad = Mathf.PI * ROS_alt / 180;
            float long_rad = Mathf.PI * ROS_long / 180;


            float x_pos = (earth_radius + alt_rad) * (float)Math.Cos(lat_rad) * (float)Math.Cos(long_rad) / ROS_to_Unity_Scale;
            float y_pos = (earth_radius + alt_rad) * (float)Math.Cos(lat_rad) * (float)Math.Sin(long_rad) / ROS_to_Unity_Scale;
            float z_pos = (earth_radius + alt_rad) * (float)Math.Sin(lat_rad) / ROS_to_Unity_Scale; */

            //Debug.LogFormat("Input: {0} {1} {2} ", ROS_lat, ROS_alt, ROS_long);
            //Debug.LogFormat("Output: {0} {1} {2} ", x_pos, y_pos, z_pos);
            //Debug.Log(ROS_to_Unity_Scale);

            //Debug.LogFormat("Input: {0}  Output: {1} ", ROS_alt, y_pos);


            //return new Vector3(x_pos, y_pos, z_pos);
            //return new Vector3(ROS_lat, ROS_alt - 100.0f, ROS_long);
            return new Vector3(ROS_lat*10000, ROS_alt - 100, ROS_long * 10000);
        }

        public static Vector3 M210_UnityToROS(float x , float y, float z)
        {
            Vector3 final_ROS_coordinates = Vector3.zero;


            float unity_long = (float)Math.Atan(y / z);
            float unity_lat = (float)Math.Atan((z / y) * (float)Math.Sin(unity_long));
            float unity_alt = z / (float)Math.Sin(unity_lat) - earth_radius;

            final_ROS_coordinates.x = unity_long;
            final_ROS_coordinates.y = unity_lat;
            final_ROS_coordinates.z = unity_alt;

            //return final_ROS_coordinates * ROS_to_Unity_Scale;
            //return new Vector3(x / 100000, y + 100.0f, z / 100000);
            return new Vector3(x / 10000, y, z / 10000);
        }


        /// <summary>
        /// Converts the worldPosition vector to the ROSPosition vector
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 WorldSpaceToRosSpace(Vector3 worldPosition)
        {
            return new Vector3(
                -worldPosition.x,
                -worldPosition.z,
                worldPosition.y - 0.148f
                );
        }

        /// <summary>
        /// Converts the ROSPosition to WorldPosition
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 RosSpaceToWorldSpace(Vector3 ROSPosition)
        {
            return new Vector3(
                -ROSPosition.x,
                ROSPosition.z + 0.148f,
                -ROSPosition.y
                );
        }

        /// <summary>
        /// Converts the ROSPosition to WorldPosition
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 RosSpaceToWorldSpace(float pose_x, float pose_y, float pose_z)
        {
            return new Vector3(
                -pose_x,
                pose_z + 0.148f,
                -pose_y
                );
        }

        /// <summary>
        /// Converts the ROSRotation to a yaw angle
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static float RosRotationToWorldYaw(float pose_x_rot, float pose_y_rot, float pose_z_rot, float pose_w_rot)
        {
            Quaternion q = new Quaternion(
                pose_x_rot,
                pose_y_rot,
                pose_z_rot,
                pose_w_rot
                );

            float sqw = q.w * q.w;
            float sqz = q.z * q.z;
            float yaw = 57.2958f * (float)Mathf.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (sqz + sqw));

            Debug.Log(yaw);
            return yaw;
        }

        public static void FindClosestObstacleAndDist()
        {

            if (WorldProperties.obstacles.Count > 0)
            {
                closestDist = Vector3.Distance(WorldProperties.selectedDrone.gameObjectPointer.transform.localPosition, WorldProperties.obstacles[0].transform.localPosition);
                closestObstacle = WorldProperties.obstacles[0];
                float dist;
                foreach (GameObject obstacle in WorldProperties.obstacles)
                {
                    dist = Vector3.Distance(WorldProperties.selectedDrone.gameObjectPointer.transform.localPosition, obstacle.transform.localPosition);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestObstacle = obstacle;
                    }
                }
            }
        }

        /// <summary>
        /// When the application closes, writes the closest obstacle distances to a text file
        /// </summary>
        void OnApplicationQuit()
        {
           
        }

        /// <summary>
        /// WriteData () writes the distances of all the instances of the closestObstacles to Assets/Results/obstacles.txt
        /// </summary>
        static void WriteData()
        {
            //Find the closest obstacle from the selected drone and its distance

            string path = "Assets/Results/obstacles.txt";

            //clear content of file 
            FileStream fileStream = File.Open(path, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();


            StreamWriter writer = new StreamWriter(path, true);
            foreach (string dist in WorldProperties.obstacleDistsToPrint)
            {
                writer.WriteLine(WorldProperties.closestDist);
            }
            writer.Close();


            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path);

            //Print the text from the file
            //Debug.Log("Text " + WorldProperties.asset.text);
        }
    }
}