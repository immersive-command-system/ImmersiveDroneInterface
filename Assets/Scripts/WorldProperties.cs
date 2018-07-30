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
        public GameObject droneBaseObject;
        public GameObject waypointBaseObject;
        public static Shader clipShader;
        public static Dictionary<char, Drone> dronesDict;
        public static Drone selectedDrone;
        public static char nextDroneId;
        public static GameObject worldObject; // Refers to the ground
        public static Vector3 actualScale;
        public static Vector3 currentScale;
        private static float maxHeight;

        public GameObject torus;
        public static List<GameObject> obstacles;
        public static TextAsset asset; //used in ROSDroneSubscriber
        public static GameObject closestObstacle;
        public static float closestDist; //between obstacle and drone
        public static HashSet<int> obstacleids; //used in ObstacleSubscriber
        public static List<string> obstacleDistsToPrint;



        // Use this for initialization
        void Start()
        {
            selectedDrone = null;
            dronesDict = new Dictionary<char, Drone>(); // Collection of all the drone classObjects
            nextDroneId = 'A'; // Used as an incrementing key for the dronesDict and for a piece of the communication about waypoints across the ROSBridge
            worldObject = gameObject;
            actualScale = new Vector3(1, 1, 1);
            currentScale = new Vector3(1, 1, 1);
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
            WriteData();
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
