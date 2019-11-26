using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.interface_msgs;


public class ISAACS_Master : MonoBehaviour {

    public DebugUISample UI_Controller;
    public GameObject Drone_GameObject;
    public ROSDroneConnection ROS_Controller;
    public Waypoint_Master Waypoint_Master_Controller;

    public List<Vector3> Waypoint_Unity = new List<Vector3>();
    public List<MissionWaypointMsg> Waypoints_ROS = new List<MissionWaypointMsg>();


    private bool AuthRequested = false; //Set true by Get_Authority_Request(). Set false by Land_Request() and Go_Home_Request(). Required to be true before any other requests can be sent.
    private bool HasTakenOff = false; //Set true by Takeoff_Request(). Set false by Land_Request() and Go_Home_Request(). Required to be true before Fly_Request(), Land_Request(), or Go_Home_Request().
    private bool waypointsPublished = false; //set true by publish
	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update () {

        //Debugging Code
        if (ROS_Controller.GetConnectionStatus())
        {
            if (Input.GetKeyUp("b"))
            {
                ROS_Controller.GetAuthority();
            }
        }


    }


    //Requests
    public void Get_Authority_Request()
    {
        //Command ROS_Controller to send GetAuthority Request to Manifold on drone
        Debug.Log("User requested auth. Calling GetAuthority() from ROS_Controller.");
        ROS_Controller.GetAuthority();
        AuthRequested = true;
    }

    public void Takeoff_Request()
    {
        //Command ROS_Controller to send GetAuthority Request to Manifold on drone
        if (AuthRequested)
        {
            Debug.Log("User requested takeoff. Calling Takeoff() from ROS_Controller.");
            ROS_Controller.Takeoff();
            HasTakenOff = true;
        } else
        {
            Debug.Log("User requested takeoff before requesting auth. Auth must be requested before takeoff. Request denied.");
        }
    }

    public void Publish_Request()
    {
        //Command ROS_Controller to send GetAuthority Request to Manifold on drone
        if (AuthRequested)
        {
            Debug.Log("User requested fly. Calling UploadMission() from ROS_Controller.");


            uint[] command_list = new uint[16];
            uint[] command_params = new uint[16];
            for (int i = 0; i < 16; i++)
            {
                command_list[i] = 0;
                command_params[i] = 0;
            }

            MissionWaypointTaskMsg Task = new MissionWaypointTaskMsg(15.0f, 15.0f, MissionWaypointTaskMsg.ActionOnFinish.RETURN_TO_HOME, 1, MissionWaypointTaskMsg.YawMode.AUTO, MissionWaypointTaskMsg.TraceMode.POINT, MissionWaypointTaskMsg.ActionOnRCLost.FREE, MissionWaypointTaskMsg.GimbalPitchMode.FREE, Waypoints_ROS.ToArray());
            ROS_Controller.UploadMission(Task);
            Debug.Log("Uploaded mission. Requesting ExecuteMission from ROS_Controller.");
            waypointsPublished = true;
          //  ROS_Controller.ExecuteMission();

        }
        else //if (!AuthRequested)
        {
            Debug.Log("User requested fly before requesting auth. auth must be requested before fly. Request denied.");
        }
       /* else
        {
            Debug.Log("User requested fly before requesting takeoff. Takeoff must be requested before land. Request denied.");
        } 
        */
    }

    public void Land_Request()
    {
        //Command ROS_Controller to send GetAuthority Request to Manifold on drone
        if (HasTakenOff)
        {
            Debug.Log("User requested land. Calling Land() from ROS_Controller.");
            // TODO : Fix Logic
            ROS_Controller.Land();
            HasTakenOff = false;
        }
        else if (!AuthRequested)
        {
            Debug.Log("User requested land before requesting auth or takeoff. Both auth and takeoff must be requested before land. Request denied.");
        }
        else
        {
            Debug.Log("User requested land before requesting takeoff. Takeoff must be requested before land. Request denied.");
        }
    }

    public void Execute_Request()
    {
        //Command ROS_Controller to send GetAuthority Request to Manifold on drone
        if (waypointsPublished)
        {
            Debug.Log("executing mission");
            ROS_Controller.ExecuteMission();
        }
       
        else
        {
            Debug.Log("waypoints not yet published");
        }
    }

    public void Go_Home_Request()
    {
        //Command ROS_Controller to send GetAuthority Request to Manifold on drone
        if (HasTakenOff)
        {
            Debug.Log("User requested go home. Calling GoHome() from ROS_Controller.");
            ROS_Controller.GoHome();
            AuthRequested = false;
            HasTakenOff = false;
        }
        else if (!AuthRequested)
        {
            Debug.Log("User requested go home before requesting auth or takeoff. Both auth and takeoff must be requested before go home. Request denied.");
        }
        else
        {
            Debug.Log("User requested go home before requesting takeoff. Takeoff must be requested before go home. Request denied.");
        }
    }

    public void Add_Waypoint(Vector3 waypoint)
    {
        Waypoint_Unity.Add(waypoint);

        // TODO : convert Unity coordinates to ROS waypoint msg

        /*
        MissionWaypointMsg helper_waypoint = new MissionWaypointMsg(); 
        Waypoints_ROS.Add(helper_waypoint);
        */
        uint[] command_list = new uint[16];
        uint[] command_params = new uint[16];
        for (int i = 0; i < 16; i++)
        {
            command_list[i] = 0;
            command_params[i] = 0;
        }
        float x = waypoint.x / 10000;
        float y = waypoint.y;
        float z = waypoint.z / 10000;
         MissionWaypointMsg new_waypoint = new MissionWaypointMsg(x,z,y, 3.0f, 0, 0, MissionWaypointMsg.TurnMode.CLOCKWISE, 0, 30, new MissionWaypointActionMsg(0, command_list, command_params));
         Waypoints_ROS.Add(new_waypoint);
         print("list: " + Waypoints_ROS);
         print("new WP: " + new_waypoint);

    }

}
