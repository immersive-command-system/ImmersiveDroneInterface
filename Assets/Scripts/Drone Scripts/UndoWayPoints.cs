namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    public class UndoWayPoints : MonoBehaviour
    {

        public static void UndoAndDeleteWaypoints(bool selectionZone, GameObject currentWaypointZone)
        {
            Drone thisDrone = WorldProperties.selectedDrone;
            //Makes sure that there are some waypoints
            if (WorldProperties.selectedDrone.waypoints != null && WorldProperties.selectedDrone.waypoints.Count > 0)
            {
                //THIS IS THE DELETE FUNCTION
                //Checking to see if the controller is near a specific waypoint
                if (selectionZone)
                {
                    Debug.Log("attempting to remove specific waypoint in zone");
                    ControllerInteractions.selectionZone = false;
                    thisDrone.gameObjectPointer.GetComponent<SetWaypoint>().ClearSpecificWayPoint(currentWaypointZone.gameObject);
                }

                //THIS IS THE UNDO FUNCTION (Delete except not in zone) 
                else
                {
                    Debug.Log("removing latest waypoint from action array");
                    thisDrone.gameObjectPointer.GetComponent<SetWaypoint>().ClearSpecificWayPoint((GameObject)thisDrone.waypointOrder[thisDrone.waypointOrder.Count - 1]);
                }

            }

            //TODO I don't think this is ever getting run because I think SpecificWayPointClear / WayPointClear are taking care of it. I could be wrong though. 
            //THIS IS THE UNDO FUNCTION if no waypoints
            else
            {
                if (thisDrone != null)
                {
                    WorldProperties.dronesDict.Remove(thisDrone.id);
                    Destroy(thisDrone.gameObjectPointer);
                }
            }
        }
    }
}
