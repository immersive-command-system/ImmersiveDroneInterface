namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SelectedDrones
    {
        Dictionary<string, Drone> individualDrones;
        Dictionary<string, Dictionary<string, Drone>> groupedDrones;

        // Use this for initialization
        public SelectedDrones(Drone initialDrone)
        {
            individualDrones[initialDrone.id] = initialDdrone;
        }



        public void AddWayPoint(Waypoint newWaypoint)
        {

        }

        public void insertWayPoint(Waypoint newWaypoint)
        {

        }

        public void deleteWayPoint(Waypoint deletedWaypoint)
        {

        }

    }
}
