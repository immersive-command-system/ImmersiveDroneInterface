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
        public ArrayList waypoints; // All waypoints held by the group as a whole

        public DroneGroup(string groupIdName, Dictionary<string, Drone> group)
        {
            groupId = groupIdName;
            color = WorldProperties.getNextGroupColor(); //getNextGroupColor needs to be implemented
            dronesDict = group;
        }

        public DroneGroup(Dictionary<string, Drone> group)
        {
            dronesDict = group;
            groupId = WorldProperties.getNextGroupId();
            color = WorldProperties.getNextGroupColor(); //getNextGroupColor needs to be implemented
        }

        public void AddWaypoint(GroupWaypoint waypoint)
        {
            waypoints.Add(waypoint);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.AddWaypoint(new Waypoint(entry.Value, waypoint.GetPosition()));
            }
        }

        public void OnModifyWaypoint(GroupWaypoint waypoint)
        {
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.OnModifyWaypoint(waypoint.GetPosition());
            }
        }

        public void DeleteWaypoint(GroupWaypoint waypoint)
        {
            if (!this.waypoints.Contains(waypoint))
            {
                return;
            }
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.DeleteWaypoint(waypoint.GetPosition());
            }
            this.waypoints.Remove(waypoint);
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
