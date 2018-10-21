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

        public bool selected = false;
        

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

        public void Ungroup(bool removeGroupWaypoints, bool deselect, bool clearIDs)
        {
            if (deselect)
            {
                Deselect();
            }
            WorldProperties.groupedDrones.Remove(this.groupId);
            if (removeGroupWaypoints)
            {
                ClearWaypoints();
            }
            if (clearIDs)
            {
                foreach (Drone drone in dronesDict.Values)
                {
                    drone.groupID = null;
                }
            }
            dronesDict.Clear();
        }

        public GroupWaypoint AddWaypoint(Vector3 position)
        {
            GroupWaypoint waypoint = new GroupWaypoint(this.groupId, position, (this.waypoints.Count > 0) ? (GroupWaypoint)this.waypoints[this.waypoints.Count - 1] : null);
            waypoints.Add(waypoint);
            Dictionary<string, Waypoint> currWaypointDict = new Dictionary<string, Waypoint>();
            individualDroneWaypoints.Add(currWaypointDict);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                Debug.Log("Creating waypoint for drone" + entry.Value.id);
                Waypoint newWaypoint = new Waypoint(entry.Value, waypoint.GetPosition());
                entry.Value.AddWaypoint(newWaypoint);
                currWaypointDict.Add(entry.Value.id, newWaypoint);
            }
            return waypoint;
        }

        public GroupWaypoint InsertWaypoint(Vector3 position, GroupWaypoint prevWaypoint)
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
                    return null;
                }
            }
            insertIndex++;

            Dictionary<string, Waypoint> currWaypointDict = new Dictionary<string, Waypoint>();
            Dictionary<string, Waypoint> prevWaypointDict = (insertIndex == 0) ? null : (Dictionary<string, Waypoint>)individualDroneWaypoints[insertIndex - 1];
            GroupWaypoint waypoint = new GroupWaypoint(this.groupId, position, prevWaypoint);
            if (insertIndex < waypoints.Count)
            {
                ((GroupWaypoint)waypoints[waypoints.Count - 1]).SetPrevWaypoint(waypoint);
            }
            waypoints.Insert(insertIndex, waypoint);
            individualDroneWaypoints.Insert(insertIndex, currWaypointDict);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                Waypoint dronePrevWaypoint = (prevWaypoint == null) ? null : prevWaypointDict[entry.Key];
                Waypoint newWaypoint = new Waypoint(entry.Value, waypoint.GetPosition());
                entry.Value.InsertWaypoint(newWaypoint, dronePrevWaypoint);
                currWaypointDict.Add(entry.Key, newWaypoint);
            }
            return waypoint;
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
            updateIndividualWaypointUI(waypoint_ind);
        }

        public void UpdateIndividualWaypointUI(GroupWaypoint waypoint)
        {
            int waypoint_ind = findWaypoint(waypoint);
            if (waypoint_ind < 0)
            {
                return;
            }
            updateIndividualWaypointUI(waypoint_ind);
        }

        private void updateIndividualWaypointUI(int waypoint_ind)
        {
            GroupWaypoint waypoint = (GroupWaypoint)waypoints[waypoint_ind];
            Dictionary<string, Waypoint> currWaypointDict = (Dictionary<string, Waypoint>)individualDroneWaypoints[waypoint_ind];
            foreach (Waypoint droneWaypoint in currWaypointDict.Values)
            {
                droneWaypoint.AssignPosition(waypoint.GetPosition());
            }
        }

        public void DeleteWaypoint(GroupWaypoint waypoint)
        {
            int waypoint_ind = findWaypoint(waypoint);
            if (waypoint_ind < 0)
            {
                return;
            }
            deleteWaypointAtIndex(waypoint_ind);
        }

        public void ClearWaypoints()
        {
            for (int i = waypoints.Count - 1; i >= 0; i--)
            {
                deleteWaypointAtIndex(i);
            }
        }

        private void deleteWaypointAtIndex(int waypoint_ind)
        {
            GroupWaypoint waypoint = (GroupWaypoint)waypoints[waypoint_ind];
            Dictionary<string, Waypoint> droneWaypointDict = (Dictionary<string, Waypoint>)(individualDroneWaypoints[waypoint_ind]);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.DeleteWaypoint(droneWaypointDict[entry.Key]);
            }
            if (waypoint_ind < this.waypoints.Count - 1)
            {
                ((GroupWaypoint)this.waypoints[waypoint_ind + 1]).SetPrevWaypoint(waypoint.GetPrevWaypoint());
            }
            this.waypoints.RemoveAt(waypoint_ind);
            this.individualDroneWaypoints.RemoveAt(waypoint_ind);
            Object.Destroy(waypoint.gameObjectPointer);
        }

        public void Select()
        {
            WorldProperties.selectedDrones.AddGroup(this);
            OnSelect();
        }

        public void OnSelect()
        {
            foreach (Drone drone in this.dronesDict.Values)
            {
                drone.OnSelect();
            }
            this.selected = true;
        }

        public void Deselect()
        {
            WorldProperties.selectedDrones.DeselectGroup(this);
            OnDeselect();
        }

        public void OnDeselect()
        {
            foreach (Drone drone in this.dronesDict.Values)
            {
                drone.OnDeselect();
            }
            this.selected = false;
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
