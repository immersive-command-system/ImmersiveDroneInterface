using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

/**
 * This is a toy example of the Unity-ROS interface talking to the TurtleSim 
 * tutorial (circa Groovy). Note that due to some changes since then this will have
 * to be slightly re-written, but as its a test ....
 * 
 * This defines the callback that responds to service requests. Try the 'T' character
 * to toggle the line left by the turtle.
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.0
 **/

public class ROSDroneServiceResponse {

    public static void ServiceCallBack(string service, string response) {
        if (response == null)
            Debug.Log("ServiceCallback for service " + service);
        else
            Debug.Log("ServiceCallback for service " + service + " response " + response);
    }
}