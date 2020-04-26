namespace ISAACS
{
    using UnityEngine;
    using ROSBridgeLib;
    using ROSBridgeLib.std_msgs;
    using ROSBridgeLib.interface_msgs;
    using SimpleJSON;
    using System.IO;
    using UnityEditor;

    /// <summary>
    /// Subscriber for the M210's GPS position data, including latitude/longitude/altitude.
    /// We use it to update the drone's Unity position, so as the drone flies in the real world,
    /// its movements are matched in the Unity interface.
    /// </summary>
    public class M210_DronePositionSubscriber : MonoBehaviour {

        //InitialGPSLat/Long/Alt store the coordinates of the drone when the Unity project first connects to the drone and receives
        //data from the subscriber. We use this to center our Unity map around this coordinate, so all Unity positions are relative
        //to this coordinate.
        public static float InitialGPSLat = float.NaN;
        public static float InitialGPSLong = float.NaN;
        public static float InitialGPSAlt = float.NaN;

        //We haven't received subscriber data yet, so initialized is false.
        public static bool initialized = false;

        public static Vector3 offsetPos = new Vector3(0.0f, 0.1f, 0.0f);
        private static Vector3 changePos;

        /// <summary>
        /// Returns the name of the ROS topic to subscribe to on the Manifold.
        /// </summary>
        public new static string GetMessageTopic()
        {
            return "/dji_sdk/gps_position";
        }
        /// <summary>
        /// Returns the ROS message which specifies the data structure of the Drone Position message.
        /// </summary>
        public new static string GetMessageType()
        {
            return "sensor_msgs/NavSatFix";
        }
        /// <summary>
        /// Returns the ROS drone position message containing information from the given JSON object.
        /// </summary>
        public new static ROSBridgeMsg ParseMessage(JSONNode msg)
        {
            return new M210_DronePositionMsg(msg);
        }
        /// <summary>
        /// CallBack is called every time the subscriber receives a new message. It updates the drone game object's
        /// position in the Unity world to match its real-world GPS position. If this is the first message received,
        /// it sets the drone's initial latitude/longitude/altitude positions.
        /// </summary>
        public new static void CallBack(ROSBridgeMsg msg)
        {

            // Get the Drone Gameobject
            GameObject robot = GameObject.FindWithTag("Drone");
            GameObject drone = WorldProperties.selectedDrone.gameObjectPointer;

            /// <summary>
            /// Upon first CallBack, save initial ROS and Unity positions.
            /// All other CallBacks result in updating the drone positions.
            /// </summary>
            if (drone != null)
            {
                /// new_ROSPosition is the ROS message containing all position data received from the drone.
                M210_DronePositionMsg new_ROSPosition = (M210_DronePositionMsg)msg;

                /// Sets the drone's initial pos when called for the first time.
                if (float.IsNaN(InitialGPSLat) && float.IsNaN(InitialGPSLong) && float.IsNaN(InitialGPSAlt))
                {
                    InitialGPSLat = new_ROSPosition._lat;
                    InitialGPSLong = new_ROSPosition._long;
                    InitialGPSAlt = new_ROSPosition._altitude;

                    //Initial position is also stored as global variables in WorldProperties.
                    WorldProperties.droneHomeLat = new_ROSPosition._lat;
                    WorldProperties.droneHomeLong = new_ROSPosition._long;
                    WorldProperties.droneHomeAlt = new_ROSPosition._altitude;
                    WorldProperties.droneInitialPositionSet = true;

                    // Initilize MapBox cityMap
                    GameObject.FindWithTag("World").GetComponent<WorldProperties>().InitializeCityMap();
                }

                /// Calculates the 3D displacement of the drone from it's initial position, to its current position, in Unity coordinates.
                changePos = new Vector3(
                    ((float) (WorldProperties.LatDiffMeters(InitialGPSLat, new_ROSPosition._lat)) / WorldProperties.Unity_X_To_Lat_Scale),
                    ((new_ROSPosition._altitude - InitialGPSAlt) / WorldProperties.Unity_Y_To_Alt_Scale),
                    ((float)(WorldProperties.LongDiffMeters(InitialGPSLong, new_ROSPosition._long, new_ROSPosition._lat) / WorldProperties.Unity_Z_To_Long_Scale))
                  );

                /// sets the drone Game Object's local position in the Unity world to be it's start position plus the newly calculated 3d displacement to the drone's current position.
                drone.transform.localPosition = WorldProperties.selectedDroneStartPos + offsetPos + changePos;
            }
            else
            {
                Debug.Log("The RosDroneSubscriber script can't find the robot.");
            }
        }


    }
}
