using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using UnityEditor;
using System.IO;

public class ROSDroneConnection : MonoBehaviour {
    private ROSBridgeWebSocketConnection ros = null;

    void Start()
    {
        Debug.Log("Attempting to create ROS connection");
        ros = new ROSBridgeWebSocketConnection("ws://192.168.0.107", 9090);
        ros.AddSubscriber(typeof(ObstacleSubscriber));
        ros.AddSubscriber(typeof(ROSDroneSubscriber));
        ros.AddPublisher(typeof(ROSDronePublisher));
        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));
        ros.Connect();
    }

    // Extremely important to disconnect from ROS. OTherwise packets continue to flow
    void OnApplicationQuit()
    {
        Debug.Log("Quiting application");
        if (ros != null)
        {
            ros.Disconnect();
        }
    }

    // Update is called once per frame in Unity
    void Update()
    {
        ros.Render();
    }

    //WriteData will write the location of the gameObject passed to it to a text file
    [MenuItem("Tools/Write file")]
    static void WriteData(GameObject robot)
    {
        string path = "Assets/Results/user_test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(robot.transform.position);
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset) Resources.Load("test");

        //Print the text from the file
        Debug.Log(asset.text);
    }

    public void PublishWaypointUpdateMessage(WaypointUpdateMsg msg)
    {
        Debug.Log("Published new waypoint message");
        ros.Publish("/waypoints", msg);
    }
}
