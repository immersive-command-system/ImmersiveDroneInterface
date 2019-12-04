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


                // First CallBack Logic
                /*if (WorldProperties.initial_DroneROS_Position == Vector3.zero)
                {
                    WorldProperties.M210_FirstPositionCallback(new_ROSPosition, drone.transform.localPosition);
                }*/

                // All CallBack Logic: Update the drone position.
                 Vector3 initial_DronePos = new Vector3(0.0f, 0.1f, 0.0f);

                //Debug.Log("Initial: " + initial_DronePos);
                
                Vector3 new_DroneUnityPositon = WorldProperties.M210_ROSToUnity(new_ROSPosition._lat, new_ROSPosition._altitude, new_ROSPosition._long);

                //Debug.Log("Callback: " + new_DroneUnityPositon);
                
                Vector3 change_DronePos = (new_DroneUnityPositon - initial_DronePos);
              //  change_DronePos.y /= 5;


                //Debug.Log("Change: " + change_DronePos);

                //change_DronePos.y = change_DronePos.y - 2.3f;

                //Debug.Log("Scaled: " + change_DronePos);

                drone.transform.localPosition = WorldProperties.selectedDroneStartPos + change_DronePos;
            }
            else
            {
                Debug.Log("The RosDroneSubscriber script can't find the robot.");
            }
        }


    }
}
