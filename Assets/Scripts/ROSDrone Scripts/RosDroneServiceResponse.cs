using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public class ROSDroneServiceResponse {

    public static void ServiceCallBack(string service, string response) {
        if (response == null)
            Debug.Log("ServiceCallback for service " + service);
        else
            Debug.Log("ServiceCallback for service " + service + " response " + response);
    }
}