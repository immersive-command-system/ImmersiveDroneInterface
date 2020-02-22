namespace ISAACS
{
    using UnityEngine;
    using ROSBridgeLib;
    using ROSBridgeLib.std_msgs;
    using ROSBridgeLib.interface_msgs;
    using SimpleJSON;
    using System.IO;
    using UnityEditor;

    public class M210_DronePositionSubscriber : MonoBehaviour {
        
        public static float InitialGPSLat = float.NaN;
        public static float InitialGPSLong = float.NaN;
        public static float InitialGPSAlt = float.NaN;
        public static bool initialized = false;
        public static Vector3 initialPos = new Vector3(0.0f, 0.1f, 0.0f);
        public static Vector3 offsetPos;
        private static Vector3 changePos;
        private static bool gotROSPosition = false;

        public new static string GetMessageTopic()
        {
            return "/dji_sdk/gps_position";
        }

        public new static string GetMessageType()
        {
            return "sensor_msgs/NavSatFix";
        }

        public new static ROSBridgeMsg ParseMessage(JSONNode msg)
        {
            return new M210_DronePositionMsg(msg);
        }

        public new static void CallBack(ROSBridgeMsg msg)
        {
            Debug.Log("Drone Position Callback");

            // Get the Drone Gameobject
            GameObject robot = GameObject.FindWithTag("Drone");
            GameObject drone = WorldProperties.selectedDrone.gameObjectPointer;

            /// <summary>
            /// Upon first CallBack, save initial ROS and Unity positions.
            /// All other CallBacks result in updating the drone positions.
            /// </summary>
            if (drone != null)
            {
                
                M210_DronePositionMsg new_ROSPosition = (M210_DronePositionMsg)msg;

                if (float.IsNaN(InitialGPSLat) && float.IsNaN(InitialGPSLong) && float.IsNaN(InitialGPSAlt))
                {
                    InitialGPSLat = new_ROSPosition._lat;
                    InitialGPSLong = new_ROSPosition._long;
                    InitialGPSAlt = new_ROSPosition._altitude;
                    Debug.Log("GPS Long: " + new_ROSPosition._long);
                    Debug.Log("GPS Lat: " +  new_ROSPosition._lat);
                }

                // First CallBack Logic
                /*if (WorldProperties.initial_DroneROS_Position == Vector3.zero)
                {
                    WorldProperties.M210_FirstPositionCallback(new_ROSPosition, drone.transform.localPosition);
                }*/

                //Debug.Log("Initial: " + initial_DronePos);

                // Non lat long conversion code
                // Vector3 new_DroneUnityPositon = WorldProperties.M210_ROSToUnity(new_ROSPosition._lat, new_ROSPosition._altitude, new_ROSPosition._long);
                // Vector3 change_DronePos = (new_DroneUnityPositon - initial_DronePos);

                changePos = new Vector3(
                    (WorldProperties.LatDiffMeters(InitialGPSLat, new_ROSPosition._lat)),// / 10,
                    (new_ROSPosition._altitude - InitialGPSAlt),// / 10,
                    (WorldProperties.LongDiffMeters(InitialGPSLong, new_ROSPosition._long, new_ROSPosition._lat))
                  );
                
                /*if (gotROSPosition == false)
                {
                    offsetPos = changePos;
                }
                gotROSPosition = true;*/

                drone.transform.localPosition = WorldProperties.selectedDroneStartPos + initialPos + changePos; // offsetPos
            }
            else
            {
                Debug.Log("The RosDroneSubscriber script can't find the robot.");
            }
        }


    }
}
