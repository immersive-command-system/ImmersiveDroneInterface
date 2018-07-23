using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using UnityEditor;
using System.IO;
using ISAACS;

public class ROSDroneConnection : MonoBehaviour
{
    private ROSBridgeWebSocketConnection ros = null;

    void Start()
    {
        // This is the IP of the linux computer that is connected to the drone.  
        ros = new ROSBridgeWebSocketConnection("ws://192.168.0.185", 9090);
        ros.AddSubscriber(typeof(ObstacleSubscriber));
        ros.AddSubscriber(typeof(ROSDroneSubscriber));
        ros.AddPublisher(typeof(UserpointPublisher));
        ros.AddServiceResponse(typeof(ROSDroneServiceResponse));
        ros.Connect();
        Debug.Log("Sending connection attempt to ROS");
    }

    // Extremely important to disconnect from ROS. OTherwise packets continue to flow
    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting from ROS");
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

    public void PublishWaypointUpdateMessage(UserpointInstruction msg)
    {
        //Debug.Log("Published new userpoint instruction: "+ msg.ToYAMLString());
        ros.Publish(UserpointPublisher.GetMessageTopic(), msg);
    }

    public void SendServiceCall(string service, string args)
    {
        Debug.Log("Calling service: " + service);
        ros.CallService(service, args);
    }

    //WriteData will write the location of the gameObject passed to it to a text file
    //[MenuItem("Tools/Write file")]
    //static void WriteData(GameObject robot)
    //{
    //    string path = "Assets/Results/user_test.txt";

    //    //Write some text to the test.txt file
    //    StreamWriter writer = new StreamWriter(path, true);
    //    writer.WriteLine(robot.transform.position);
    //    writer.Close();

    //    //Re-import the file to update the reference in the editor
    //    AssetDatabase.ImportAsset(path);
    //    TextAsset asset = (TextAsset) Resources.Load("test");

    //    //Print the text from the file
    //    Debug.Log(asset.text);
    //}
}

