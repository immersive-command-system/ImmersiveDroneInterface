namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class DroneGroup
    {

        private Dictionary<string, Drone> dronesDict;
        public string groupId;
        public Color color;
        

        // The following fields are for the purpose of having the group keep track of its waypoints.
        // This makes displaying a group's waypoints easier.
        public ArrayList waypoints = new ArrayList(0); // All waypoints held by the group as a whole
        // Each element of individualDroneWaypoints should be a mapping from drone ID to the individual
        // Waypoint object for that drone corresponding to the group waypoint at the same index.
        public ArrayList individualDroneWaypoints = new ArrayList(0);

        public DroneGroup(string groupIdName, Dictionary<string, Drone> group)
        {
            groupId = groupIdName;
            dronesDict = group;
            color = WorldProperties.getNextGroupColor(); //getNextGroupColor needs to be implemented
        }

        public DroneGroup(Dictionary<string, Drone> group) : this(WorldProperties.getNextGroupId(), group) {}

        public void AddWaypoint(GroupWaypoint waypoint)
        {
            waypoints.Add(waypoint);
            Dictionary<string, Waypoint> currWaypointDict = new Dictionary<string, Waypoint>();
            individualDroneWaypoints.Add(currWaypointDict);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                Waypoint newWaypoint = new Waypoint(entry.Value, waypoint.GetPosition());
                entry.Value.AddWaypoint(newWaypoint);
                currWaypointDict.Add(entry.Value.id, newWaypoint);
            }
        }

        public void InsertWaypoint(GroupWaypoint waypoint, GroupWaypoint prevWaypoint)
        {
            int insertIndex;
            if (prevWaypoint == null)
            {
                insertIndex = -1;
            } else
            {
                insertIndex = findWaypoint(prevWaypoint);
                if (insertIndex < 0)
                {
                    return;
                }
            }
            insertIndex++;

            Dictionary<string, Waypoint> currWaypointDict = new Dictionary<string, Waypoint>();
            Dictionary<string, Waypoint> prevWaypointDict = (insertIndex == 0) ? null : (Dictionary<string, Waypoint>)individualDroneWaypoints[insertIndex - 1];
            waypoints.Insert(insertIndex, waypoint);
            individualDroneWaypoints.Insert(insertIndex, currWaypointDict);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                Waypoint dronePrevWaypoint = (prevWaypoint == null) ? null : prevWaypointDict[entry.Key];
                Waypoint newWaypoint = new Waypoint(entry.Value, waypoint.GetPosition());
                entry.Value.InsertWaypoint(newWaypoint, dronePrevWaypoint);
                currWaypointDict.Add(entry.Key, newWaypoint);
            }
        }

        public void OnModifyWaypoint(GroupWaypoint waypoint)
        {
            int waypoint_ind = findWaypoint(waypoint);
            if (waypoint_ind < 0)
            {
                return;
            }
            Dictionary<string, Waypoint> droneWaypointDict = (Dictionary<string, Waypoint>)(individualDroneWaypoints[waypoint_ind]);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.OnModifyWaypoint(droneWaypointDict[entry.Key]);
            }
        }

        public void DeleteWaypoint(GroupWaypoint waypoint)
        {
            int waypoint_ind = findWaypoint(waypoint);
            if (waypoint_ind < 0)
            {
                return;
            }
            Dictionary<string, Waypoint> droneWaypointDict = (Dictionary<string, Waypoint>)(individualDroneWaypoints[waypoint_ind]);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.DeleteWaypoint(droneWaypointDict[entry.Key]);
            }
            this.waypoints.RemoveAt(waypoint_ind);
            this.individualDroneWaypoints.RemoveAt(waypoint_ind);
        }

        private int findWaypoint(GroupWaypoint waypoint)
        {
            for (int i = 0; i < this.waypoints.Count; i++)
            {
                if (this.waypoints[i] == waypoint)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<string> getIDs()
        {
            return dronesDict.Keys.ToList();
        }

        public List<Drone> getDrones()
        {
            return dronesDict.Values.ToList();
        }

        public Drone getDrone(string id)
        {
            return dronesDict[id];
        }

        public Dictionary<string, Drone> getDronesDict()
        {
            return dronesDict;
        }
        
    }
}
