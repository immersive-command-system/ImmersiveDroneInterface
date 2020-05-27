using UnityEngine;
using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
using UnityEngine.SceneManagement;
using System.Collections;
using VRTK;
using ISAACS;

public class DroneButtons : MonoBehaviour {

    Button myButton;
    Drone drone;
    private GameObject controller; //needed to access pointer

    public bool simulation = true;

    public bool startMission = false;
    public bool pauseMission = false;
    public bool resumeMission = false;
    public bool clearWaypoints = false;
    public bool landDrone = false;
    public bool homeDrone = false;


    void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        myButton = GetComponent<Button>(); // <-- you get access to the button component here
        myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
    }

    void OnClickEvent()
    {
        if (startMission)
        {
            Debug.Log("Start Mission Button");

            if (simulation)
            {
                GameObject world = GameObject.FindGameObjectWithTag("World");
                DroneFlyingDemo drone = world.GetComponent<DroneFlyingDemo>();
                drone.FlyNextWaypoint(true);
                return;
            }

            // Static Waypoint system:
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().CreateMission();

            // Test and switch to dynamic waypoint system
            WorldProperties.StartDroneMission();
        }

        if (pauseMission)
        {
            Debug.Log("TO TEST: Pause Mission Button");

            if (simulation)
            {
                GameObject world = GameObject.FindGameObjectWithTag("World");
                DroneFlyingDemo drone = world.GetComponent<DroneFlyingDemo>();
                drone.pauseFlight();
                return;
            }
            // Test and switch
            // WorldProperties.PauseDroneMission();
        }

        if (resumeMission)
        {
            Debug.Log("TO TEST: Resume Mission Button");
            if (simulation)
            {
                GameObject world = GameObject.FindGameObjectWithTag("World");
                DroneFlyingDemo drone = world.GetComponent<DroneFlyingDemo>();
                drone.resumeFlight();
                return;
            }

            // Test and switch
            // WorldProperties.ResumeDroneMission();
        }


        if (clearWaypoints)
        {
            Debug.Log(" Clear Waypoints  Button");
            if (controller.GetComponent<VRTK_Pointer>().IsPointerActive())
            {
                drone = WorldProperties.selectedDrone;
                while (drone.waypoints.Count > 1)
                {
                    if (((Waypoint)drone.waypoints[drone.waypoints.Count - 1]).prevPathPoint != null)
                    {
                        drone.DeleteWaypoint((Waypoint)drone.waypoints[drone.waypoints.Count - 1]);
                    }
                }
            }
        }


        if (landDrone)
        {
            Debug.Log("Land  Button");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().Land();
        }


        if (homeDrone)
        {
            Debug.Log("Home  Button");

            if (simulation)
            {
                GameObject world = GameObject.FindGameObjectWithTag("World");
                DroneFlyingDemo drone = world.GetComponent<DroneFlyingDemo>();
                drone.flyHome();
                return;
            }

            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().GoHome();
        }


    }
}
