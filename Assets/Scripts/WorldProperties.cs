namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

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
        /// Converts the ROS Position given in lat, long, and alt to WorldPosition
        /// </summary>
        /// <param name="pose_x"></param>
        /// <param name="pose_y"></param>
        /// <param name="pose_z"></param>
        /// <returns></returns>
        public static Vector2 M210_RosSpaceToWorld(float _lat, float _long, float _alt)
        {
            return new Vector3(
                _lat,
                _alt,
                _long
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