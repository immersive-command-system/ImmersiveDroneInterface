using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using ISAACS;
using System.IO;
using UnityEditor;

public class ROSDroneSubscriber : ROSBridgeSubscriber
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
        //Debug.Log("callback");
        GameObject robot = GameObject.FindWithTag("Drone");
        if (robot != null)
        {
            DronePositionMsg pose = (DronePositionMsg)msg;
            Vector3 tablePos = GameObject.FindWithTag("Table").transform.position;
            robot.transform.localPosition = new Vector3(-pose._x, pose._z + tablePos.z + 0.148f, -pose._y);
            //Debug.Log(robot.transform.position);
            //robot.transform.rotation = Quaternion.AngleAxis(-pose.getTheta() * 180.0f / 3.1415f, Vector3.up);
        } else {
            //Debug.Log("The RosDroneSubscriber script can't find the robot.");
        }
    }

    //WriteData will write the location of the closest Obstacle passed to it to a text file
    //[MenuItem("Tools/Write file")]
    static void WriteData(GameObject robot)
    {
        //Find the closest obstacle from drone and its distance
        GameObject closestObstacle;
        float closestDist;
        FindClosestObstacleAndDist(out closestObstacle, out closestDist); //if no obstacles exist, closestDist is -1, and closestObstacle is null       
        string path = "Assets/Results/obstacles.txt";

        Debug.Log("hi");

        if (closestObstacle != null)
        {
            Debug.Log(closestObstacle.name + ": " + closestDist);

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(closestObstacle.name + ": " + closestDist);
            writer.Close();

            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path);
            TextAsset asset = (TextAsset)Resources.Load("test");

            //Print the text from the file
            Debug.Log(asset.text);
        }

        
    }

    static void FindClosestObstacleAndDist(out GameObject closestObstacle, out float closestDist)
    {
        closestObstacle = null;
        closestDist = -1;
        if (ObstacleSubscriber.obstacles.Count > 0)
        {
            closestDist = Vector3.Distance(WorldProperties.selectedDrone.gameObjectPointer.transform.localPosition, ObstacleSubscriber.obstacles[0].transform.localPosition);
            float dist;
            foreach (GameObject obstacle in ObstacleSubscriber.obstacles)
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
}