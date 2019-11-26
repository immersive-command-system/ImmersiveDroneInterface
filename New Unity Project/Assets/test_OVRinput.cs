using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_OVRinput : MonoBehaviour {


    public OVRInput.Controller controller_left;
    public OVRInput.Controller controller_right;


    private float indexTriggerState = 0;
    private float handTriggerState = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float indexTriggerState_l = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller_left);
        float handTriggerState_l = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller_left);
        print("Index State: " + indexTriggerState_l);
        print("Hand State: " + handTriggerState_l);

        indexTriggerState = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller_right);
        handTriggerState = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller_right);
        print("Index State: " + indexTriggerState);
        print("Hand State: " + handTriggerState);
    }
}
