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

        public static Vector3 offsetPos = new Vector3(0.0f, 0.1f, 0.0f);
        private static Vector3 changePos;

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
            //Debug.Log("Drone Position Callback");

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

                    WorldProperties.droneHomeLat = new_ROSPosition._lat;
                    WorldProperties.droneHomeLong = new_ROSPosition._long;
                    WorldProperties.droneHomeAlt = new_ROSPosition._altitude;
                    WorldProperties.droneInitialPositionSet = true;


                    Debug.Log("GPS Long: " + new_ROSPosition._long);
                    Debug.Log("GPS Lat:  " +  new_ROSPosition._lat);
                    Debug.Log("GPS Alt:  " + new_ROSPosition._altitude);


                    // Initilize cityMap
                    // Peru: 3/7/2020 : Map Integration
                    GameObject.FindWithTag("World").GetComponent<WorldProperties>().InitializeCityMap();
                }

                // Non lat long conversion code
                // Vector3 new_DroneUnityPositon = WorldProperties.M210_ROSToUnity(new_ROSPosition._lat, new_ROSPosition._altitude, new_ROSPosition._long);
                // Vector3 change_DronePos = (new_DroneUnityPositon - initial_DronePos);
                //Debug.Log("initial gps alt: " + InitialGPSAlt);
                //Debug.Log("current gps alt: " + new_ROSPosition._altitude);
                changePos = new Vector3(
                    ((float) (WorldProperties.LatDiffMeters(InitialGPSLat, new_ROSPosition._lat)) / WorldProperties.Unity_X_To_Lat_Scale),
                    ((new_ROSPosition._altitude - InitialGPSAlt) / WorldProperties.Unity_Y_To_Alt_Scale),
                    ((float)(WorldProperties.LongDiffMeters(InitialGPSLong, new_ROSPosition._long, new_ROSPosition._lat) / WorldProperties.Unity_Z_To_Long_Scale))
                  );

               // Debug.Log("----");
                //Debug.Log(new_ROSPosition._lat + ":" + new_ROSPosition._long + ":" + new_ROSPosition._altitude);
                drone.transform.localPosition = WorldProperties.selectedDroneStartPos + offsetPos + changePos; // offsetPos
                                                                                                               //Debug.Log(drone.transform.localPosition);
                                                                                                               //Debug.Log("----");

            }
            else
            {
                Debug.Log("The RosDroneSubscriber script can't find the robot.");
            }
        }


    }
}
