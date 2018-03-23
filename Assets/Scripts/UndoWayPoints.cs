using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UndoWayPoints : MonoBehaviour {

	public static void UndoAndDeleteWaypoints(bool selectionZone,  GameObject currentWaypointZone)
    {
        //Makes sure that there are some waypoints
        if (SetWaypoint.waypoints.Count > 0)
        {
            //THIS IS THE DELETE FUNCTION
            //Checking to see if the controller is near a specific waypoint
            if (selectionZone)
            {
                Debug.Log("attempting to remove specific waypoint in zone");
                ControllerInteractions.selectionZone = false;
                SetWaypoint.ClearSpecificWayPoint(currentWaypointZone.gameObject);
            }

            //THIS IS THE UNDO FUNCTION (Delete except not in zone) 
            else
            {
                Debug.Log("removing latest waypoint from action array");
                SetWaypoint.ClearSpecificWayPoint((GameObject)SetWaypoint.waypointOrder[SetWaypoint.waypointOrder.Count - 1]);
                

            }

        }


        //THIS IS THE UNDO FUNCTION if no waypoints
        else
        {
            Debug.Log("Undo: Delete Drone");
            GameObject tempDrone = SetWaypoint.getCurrentDrone();
            if (tempDrone != null)
            {
                Destroy(tempDrone);
            }
            SetWaypoint.waypointOrder = new ArrayList(0);
        }
    }
}
