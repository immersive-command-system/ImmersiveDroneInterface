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
    using Mapbox.Unity.Map;
    using Mapbox.Utils;

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

        public static double droneHomeLat;
        public static double droneHomeLong;
        public static float droneHomeAlt;

        public static bool droneInitialPositionSet = false;


        public static Shader clipShader;

        public static Dictionary<char, Drone> dronesDict;
        public static Dictionary<char, GameObject> hoopsDict;

        public static Drone selectedDrone;
        public static Vector3 selectedDroneStartPos;

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

        // M210 ROS-Unity conversion variables
        public static float earth_radius = 6378137;
        public static Vector3 initial_DroneROS_Position = Vector3.zero;
        public static Vector3 initial_DroneUnity_Position = Vector3.zero;
        public static float ROS_to_Unity_Scale = 0.0f;

        //Unity to lat --> multiply by scale; lat to Unity --> divide by scale
        public static float Unity_X_To_Lat_Scale = 10.0f;
        public static float Unity_Y_To_Alt_Scale = 10.0f;
        public static float Unity_Z_To_Long_Scale = 10.0f;


        // Peru: 3/7/2020 : Map Integration Move
        // Mapbox Interactions
        public GameObject citySim;
        public AbstractMap abstractMap;
        public float initZoom_citySim = 21.0f;
        public double initLat_citySim;
        public double initLong_citySim;


        // Peru: 3/7/2020 : Map Integration
        public void InitializeCityMap()
        {
            // MapBox Initial Variables

            initLat_citySim = WorldProperties.droneHomeLat;
            initLong_citySim = WorldProperties.droneHomeLong;

            Vector2d intiLatLong = new Vector2d(WorldProperties.droneHomeLat, WorldProperties.droneHomeLong);
            abstractMap.Initialize(intiLatLong, (int)initZoom_citySim);

            this.GetComponent<MapInteractions>().citySimActive = true;
            this.GetComponent<MapInteractions>().initZoom_citySim = initZoom_citySim;

            this.GetComponent<MapInteractions>().initLat_citySim = initLat_citySim;
            this.GetComponent<MapInteractions>().initLong_citySim = initLong_citySim;
            this.GetComponent<MapInteractions>().initPosition_citySim = citySim.transform.position;

            this.GetComponent<MapInteractions>().currLat_citySim = initLat_citySim;
            this.GetComponent<MapInteractions>().currLong_citySim = initLong_citySim;
            this.GetComponent<MapInteractions>().currPosition_citySim = citySim.transform.position;

        }

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

            // Peru: 3/7/2020 : Map Integration Move
            if (Input.GetKeyUp("p"))
            {
                WorldProperties.droneHomeLat = 37.91532757;
                WorldProperties.droneHomeLong = -122.33805556;
                InitializeCityMap();
            }
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

                Debug.Log("Initializing drone");
                Drone newDrone = new Drone(worldObject.transform.position);
                selectedDrone = newDrone;
                selectedDroneStartPos = newDrone.gameObjectPointer.transform.localPosition;
                
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


        /* Deprecated as of 2/20/2020; changed from scaled conversion to latitude/longitude conversion. See LatDiffMeters(float lat1, float lat2) and LongDiffMeters(float long1, float long2, float lat) for more info. -Varun,Shreyas */
        public static Vector3 M210_ROSToUnity(float ROS_lat, float ROS_alt, float ROS_long)
        {
            return new Vector3(ROS_lat * 10000, (ROS_alt - 100) / 5, ROS_long * 10000);
        }

        /* Deprecated as of 2/20/2020; changed from scaled conversion to latitude/longitude conversion. See UnityXToLat(float lat1, float unityXCoord) and UnityZToLong(float long1, float lat, float unityZCoord) for more info. -Varun,Shreyas */
        public static Vector3 M210_UnityToROS(float x, float y, float z)
        {
            //return new Vector3(x / 100000, y + 100.0f, z / 100000);
            return new Vector3(x / 10000, y * 5, z / 10000);
        }

        /* Deprecated as of 2/20/2020; changed from scaled conversion to latitude/longitude conversion. See LatDiffMeters(float lat1, float lat2) and LongDiffMeters(float long1, float long2, float lat) for more info. -Varun,Shreyas */
        public static Vector3 M210_ROSToUnityLocal(float x, float y, float z)
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
            return new Vector3(x, y, z);
        }


        /// <summary>
        /// Converts the worldPosition vector to the ROSPosition vector
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        /// 
        /* Deprecated as of 2/20/2020; changed from scaled conversion to latitude/longitude conversion. See UnityXToLat(float lat1, float unityXCoord) and UnityZToLong(float long1, float lat, float unityZCoord) for more info. -Varun,Shreyas */
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
        /// 
        /* Deprecated as of 2/20/2020; changed from scaled conversion to latitude/longitude conversion. See UnityXToLat(float lat1, float unityXCoord) and UnityZToLong(float long1, float lat, float unityZCoord) for more info. -Varun,Shreyas */
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
        /* Deprecated as of 2/20/2020; changed from scaled conversion to latitude/longitude conversion. See UnityXToLat(float lat1, float unityXCoord) and UnityZToLong(float long1, float lat, float unityZCoord) for more info. -Varun,Shreyas */
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

        /*
        public static float latLongtoMetersConverter(float lat1, float long1, float alt1, float lat2, float long2, float alt2)
        {

            const float Rad = 6378.137f;
            float dLat = (float) (lat2 * Math.PI / 180 - lat1 * Math.PI / 180);
            float dLong = (float) (long2 * Math.PI / 180 - long1 * Math.PI / 180);
            float a = (float) (Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLong / 2) * Math.Sin(dLong/2));
            float c = (float) (2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));
            float d = (float) (Rad * c);
            return (float) (Math.Sqrt(Math.Pow(d * 1000, 2) + Math.Pow((alt2 - alt1), 2)));
        }
        */

        public static double LatDiffMeters(double lat1, double lat2)
        {
            // assuming earth is a sphere with c = 40075km
            // 1 degree of latitude is = 111.32 km
            //slight inaccuracies
            double delLat = (lat2 - lat1) * 111.32f * 1000;
            //110994.04016313434
            return delLat;
        }

        public static double LongDiffMeters(double long1, double long2, double lat)
        {
            // 1 degree of longitude = 40075 km * cos (lat) / 360
            // we use an arbitrary latitude for the conversion because the difference is minimal 
            //slight inaccuracies
            double delLong = (long2 - long1) * 40075 *(double)Math.Cos(lat) / 360 * 1000;
            return delLong;
        }

        public static double UnityXToLat(double lat1, float unityXCoord)
        {
            double delLat = (unityXCoord / (1000 * 111.32f) * Unity_X_To_Lat_Scale) + lat1;
            return delLat;
        }

        public static double UnityZToLong(double long1, double lat, float unityZCoord)
        {
            double delLong = (((unityZCoord * 360) / (1000 * 40075 * (double)Math.Cos(lat))) * Unity_Z_To_Long_Scale) + long1;
            return delLong;
        }
    }
}