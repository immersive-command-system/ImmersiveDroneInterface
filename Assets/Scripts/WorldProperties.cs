namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// This is the only class that should have static variables or functions that are consistent throughout the entire program.
    /// </summary>
    public class WorldProperties : MonoBehaviour
    {
        public GameObject droneBaseObject;
        public GameObject waypointBaseObject;
        public GameObject torus;

        public static Color droneSelectionColor;

        public static Shader clipShader;

        public static Dictionary<string, Drone> dronesDict;
        public static Dictionary<char, GameObject> hoopsDict;

        public static Dictionary<string, DroneGroup> groupedDrones;
        public static SelectedDrones selectedDrones;

        public static GameObject worldObject;
        public static GameObject placementPlane;

        public static Vector3 actualScale;
        public static Vector3 currentScale;

        public static Vector3 droneModelOffset;
        public static Vector3 torusModelOffset;

        private static float maxHeight;
        private static IEnumerator<string> nextDroneId;
        private static int nextGroupIDNum;
        private static Color nextGroupColor;

        public static List<GameObject> obstacles;
        public static TextAsset asset; //used in ROSDroneSubscriber
        public static GameObject closestObstacle;
        public static float closestDist; //between obstacle and drone
        public static HashSet<int> obstacleids; //used in ObstacleSubscriber
        public static List<string> obstacleDistsToPrint;

        // Use this for initialization
        void Start()
        {
            selectedDrones = new SelectedDrones();
            dronesDict = new Dictionary<string, Drone>(); // Collection of all the drone classObjects
            hoopsDict = new Dictionary<char, GameObject>(); // Collection of all the hoop gameObjects
            groupedDrones = new Dictionary<string, DroneGroup>(); // Collection of all the groups of drones.
            nextDroneId = generateNextDroneID().GetEnumerator(); // Used as an incrementing key for the dronesDict and for a piece of the communication about waypoints across the ROSBridge
            nextDroneId.MoveNext();
            nextGroupIDNum = 0; //used for grouped drone IDs (format is "Group" + groupIDNum)
            nextGroupColor = Color.blue; //arbitray right now; used so users can differentiate groups of drones
            droneSelectionColor = Color.yellow;

            worldObject = gameObject;
            placementPlane = GameObject.FindWithTag("Ground");
            actualScale = new Vector3(1, 1, 1);
            currentScale = new Vector3(1, 1, 1);

            droneModelOffset = new Vector3(0.0044f, -0.0388f, 0.0146f);
            torusModelOffset = new Vector3(0f, -0.3175f, 0f);

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
            }
        }
        /// <summary>
        /// Used to generate new Drone ID strings; "A" -> "Z", then "AA" -> "AZ", then "BA" -> "BZ" and so on
        /// </summary>
        /// <param name="start"></param>
        /// <returns>return an IEnumerable for generating new Drone IDs</returns>
        public static IEnumerable<string> generateNextDroneID(string start = "")
        {
            StringBuilder chars = (start == null) ? new StringBuilder() : new StringBuilder(start);

            while (true)
            {
                int i = chars.Length - 1;
                while (i >= 0 && chars[i] == 'Z')
                {
                    chars[i] = 'A';
                    i--;
                }
                if (i == -1)
                    chars.Insert(0, 'A');
                else
                    chars[i]++;
                yield return chars.ToString();
            }
        }

        /// <summary>
        /// returns the next Drone Id
        /// </summary>
        /// <returns>next Drone ID; which is in the infinite set {"A", "B", ..., "Z", "AA", ..., "AZ", "BA", ...}</returns>
        public static string getNextDroneId()
        {
            nextDroneId.MoveNext();
            string toReturn = nextDroneId.Current;
            return toReturn;
        }

        public static string getNextGroupId()
        {
            int idNum = nextGroupIDNum;
            nextGroupIDNum++;
            return "Group" + idNum;
        }

        //needs to be implemented
        public static Color getNextGroupColor()
        {
            return nextGroupColor;
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

        public static void FindClosestObstacleAndDist(Drone drone)
        {

            if (WorldProperties.obstacles.Count > 0)
            {
                closestDist = Vector3.Distance(drone.gameObjectPointer.transform.position, WorldProperties.obstacles[0].transform.localPosition);
                closestObstacle = WorldProperties.obstacles[0];
                float dist;
                foreach (GameObject obstacle in WorldProperties.obstacles)
                {
                    dist = Vector3.Distance(drone.gameObjectPointer.transform.position, obstacle.transform.localPosition);
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
