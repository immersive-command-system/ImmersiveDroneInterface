using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using ISAACS;
using System.IO;
using UnityEditor;

public class DronePositionSubscriber : ROSBridgeSubscriber
{

    public new static string GetMessageTopic()
    {
        return "/state/position_velocity";
    }

    public new static string GetMessageType()
    {
        return "crazyflie_msgs/PositionVelocityStateStamped";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new DronePositionMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("Drone Position Callback");

        GameObject robot = GameObject.FindWithTag("Drone");
        if (robot != null)
        {
            DronePositionMsg pose = (DronePositionMsg)msg;

            robot.transform.localPosition = WorldProperties.RosSpaceToWorldSpace(pose._x, pose._y, pose._z);
                //+  WorldProperties.droneModelOffset;

            //Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
            //robot.transform.localPosition = new Vector3(-pose._x, pose._z + tablePos.z + 0.148f, -pose._y);

            SaveData();

        } else {
            Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }
    }

    //WriteData will write the location of the closest Obstacle passed to it to a text file
    //[MenuItem("Tools/Write file")]
    /*static void WriteData()
    {
        //Find the closest obstacle from the selected drone and its distance

        WorldProperties.FindClosestObstacleAndDist(); //if no obstacles exist, closestDist is -1, and closestObstacle is null       
        string path = "Assets/Results/obstacles.txt";
        Debug.Log("hi");
        if (WorldProperties.closestObstacle != null)
        {
           // Debug.Log(closestObstacle.name + ": " + closestDist);

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(WorldProperties.closestObstacle.name + ": " + WorldProperties.closestDist);
            writer.Close();

            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path);
            

            //Print the text from the file
            //Debug.Log("Text " + WorldProperties.asset.text);
        }
    }
        */
    /// <summary>
    /// Finds and keeps track of all the closest obstacle distancecs as strings in a list 
    /// </summary>
    static void SaveData()
    {
        WorldProperties.FindClosestObstacleAndDist();
        WorldProperties.obstacleDistsToPrint.Add(WorldProperties.closestDist.ToString());
    }

        

}