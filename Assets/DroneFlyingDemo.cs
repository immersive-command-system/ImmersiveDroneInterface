using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISAACS;

public class DroneFlyingDemo : MonoBehaviour {

    public GameObject drone;
    public float speed = 1.0f;

    private int nextWaypointID = 0;
    private bool flying = false;
    private Vector3 origin;
    private Vector3 destination;


	// Use this for initialization
	void Start () {
        drone = WorldProperties.selectedDrone.gameObjectPointer;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FlyNextWaypoint();
        }


        if (flying)
        {
            drone.transform.localPosition = Vector3.Lerp(origin, destination, speed * Time.deltaTime);
        }

        if (flying && Vector3.Distance(drone.transform.position, destination) < 0.5f)
        {
            flying = false;
        }
    }

    void FlyNextWaypoint()
    {
        ArrayList waypoints = WorldProperties.selectedDrone.waypoints;

        /// Check if there is another waypoint
        if (waypoints.Count == nextWaypointID)
        {
            Debug.Log("ALERT: All waypoints successfully send");
            Debug.Log("ALERT: Drone is send home by default");
            flying = false;
            return;
        }

        Waypoint waypoint = (Waypoint)waypoints[nextWaypointID];      
        origin = drone.transform.localPosition;
        destination = waypoint.gameObjectPointer.transform.localPosition;
        flying = true;
         
    }

}
