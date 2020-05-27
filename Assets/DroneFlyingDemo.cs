using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISAACS;

public class DroneFlyingDemo : MonoBehaviour
{

    public Drone drone;
    public float speed = 1.0f;

    private int nextWaypointID = 0;
    private bool flying = false;
    private Vector3 origin;
    private Vector3 destination;
    private Vector3 home;
    private bool endFlight = false;
    private float fraction = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Init drone");
            drone = WorldProperties.selectedDrone;
            home = drone.gameObjectPointer.transform.localPosition;
            Debug.Log(drone);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("test waypoint");
            Vector3 test_waypoint = new Vector3(1.0f, 1.0f, 1.0f);
            FlyNextWaypoint(test_waypoint);
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Start flying");
            FlyNextWaypoint();
        }


        if (flying) 
        {
            if (fraction < 1)
            {
                fraction += Time.deltaTime * speed;
                Vector3 new_position = Vector3.Lerp(origin, destination, fraction);
                drone.gameObjectPointer.transform.localPosition = new_position;
            }
            else
            {
                flying = false;
                if (!endFlight)
                {
                    FlyNextWaypoint();
                }
            }

        }
        
    }

    public void FlyNextWaypoint(bool restart=false)
    {
        ArrayList waypoints = WorldProperties.selectedDrone.waypoints;

        if (restart)
        {
            endFlight = false;
            nextWaypointID = 0;
        }

        /// Check if there is another waypoint
        if (waypoints.Count == nextWaypointID)
        {
            Debug.Log("ALERT: All waypoints successfully send");
            Debug.Log("ALERT: Drone is send home by default");
            flying = false;
            return;
        }

        Waypoint waypoint = (Waypoint)waypoints[nextWaypointID];
        origin = drone.gameObjectPointer.transform.localPosition;
        destination = waypoint.gameObjectPointer.transform.localPosition;
        flying = true;
        nextWaypointID += 1;
        fraction = 0.0f;
        Debug.Log(nextWaypointID);
    }

    public void FlyNextWaypoint(Vector3 waypoint)
    {
        origin = drone.gameObjectPointer.transform.localPosition;
        destination = waypoint;

        Debug.Log("Origin: " + origin);
        Debug.Log("Dest:   " + destination);

        flying = true;
    }

    public void pauseFlight()
    {
        flying = false;
    }

    public void resumeFlight()
    {
        flying = true;
    }

    public void flyHome()
    {
        endFlight = true;
        FlyNextWaypoint(home);
    }

}
