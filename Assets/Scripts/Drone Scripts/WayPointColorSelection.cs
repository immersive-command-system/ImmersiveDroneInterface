using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointColorSelection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void AddWayPointColor (bool selectionZone, GameObject currentWayPointZone)
    {
        Debug.Log("ONE");
        if (selectionZone == false || currentWayPointZone == null)
        {
            Debug.Log("TWO");
            return;
        }
        Debug.Log("THREE");
        GameObject currentWaypoint = currentWayPointZone.gameObject;
        GameObject colorSphere = Instantiate(Resources.Load("Flare")) as GameObject; //Change GameObject Prefab Here
        colorSphere.transform.position = currentWaypoint.transform.position;
        colorSphere.transform.parent = currentWaypoint.transform;
        colorSphere.tag = "selectionZone";
    }

    public static void RemoveWayPointColor(bool selectionZone, GameObject currentWayPointZone)
    {
        if (selectionZone == false || currentWayPointZone == null)
        {
            return;
        }
        GameObject currentWaypoint = currentWayPointZone.gameObject;
        Transform[] childTransforms = currentWayPointZone.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.gameObject.tag == "selectionZone")
            {
                Destroy(childTransform.gameObject);
            }
        }

    }
}
