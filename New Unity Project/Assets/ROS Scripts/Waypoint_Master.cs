using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;

public class Waypoint_Master : MonoBehaviour {


    public ISAACS_Master ISAACS_master_controller;
    public GameObject waypoint_prefab;

    public OVRInput.Controller controller_left;
    public GameObject hand_left;
    public OVRInput.Controller controller_right;
    public GameObject hand_right;


    private float indexTriggerState_R = 0;
    private float indexTriggerState_L = 0;

    private bool placing = false;
    List<MissionWaypointMsg> waypoint_list = new List<MissionWaypointMsg>();

    //private WaypointActionStatus right = WaypointActionStatus.Off;
    //private WaypointActionStatus left = WaypointActionStatus.Off;

    // Use this for initialization
    void Start () {
        

    }

    // Update is called once per frame
    void Update()
    {
        
        // Define triggers; they need to be in Update(), otherwise pressing them doesn't seem to do something (no message in the log)
        indexTriggerState_R = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller_right);
        indexTriggerState_L = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller_left);

        // Start placing with the Right controller
        if (indexTriggerState_R > 0.9f)
        {
            Debug.Log(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
            placing = true;
        }

        if (placing)
        {
            if (indexTriggerState_R < 0.1f)
            {
                
                Vector3 hand_pos = hand_right.transform.position;
                Instantiate(waypoint_prefab, hand_right.transform.position, hand_right.transform.rotation);
                placing = false;
                ISAACS_master_controller.Add_Waypoint(hand_pos);
               
            }    
        }

        //print("Index State: " + indexTriggerState_R);
        //print("hand tranform: " + hand_right.transform.position);

    }

}
