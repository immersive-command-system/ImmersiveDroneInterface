namespace ISAACS
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This class is a container for a collection of drones representing a group.
    /// This implementation is currently an immutable group (no adding or removing drones).
    /// Inherent challenges for having a mutable group would be handling the group waypoints.
    /// In other words, do new additions to the group also receive older group waypoints?
    /// </summary>
    public class DroneGroup
    {

        private Dictionary<string, Drone> dronesDict;   // All drones in the selection, mapped from ID to Drone object.
        public string groupId;                          // Unique ID of the group.
        public Color color;                             // Color representing the group in the UI.

        public bool selected = false;                   // Whether group is currently in the selection.
        
        public List<GroupWaypoint> waypoints = new List<GroupWaypoint>(0); // All waypoints held by the group as a whole
        // Each element of individualDroneWaypoints is a mapping from drone ID to the that drone's
        // Waypoint object corresponding to the group waypoint at the same index in waypoints.
        public List<Dictionary<string, Waypoint>> individualDroneWaypoints = new List<Dictionary<string, Waypoint>>(0);



        /// <summary>
        /// Constructor method for DroneGroup class object
        /// </summary>
        /// <param name="groupID"> The unique ID the group will have </param>
        /// <param name="group"> The dictionary (drone ID --> Drone object) that the group will have. </param>
        public DroneGroup(string groupID, Dictionary<string, Drone> group)
        {
            groupId = groupID;
            dronesDict = group;
            color = WorldProperties.getNextGroupColor(); //getNextGroupColor needs to be implemented
        }

        /// <summary>
        /// Alternate constructor for DroneGroup class object.
        /// Will auto-obtain drone group ID.
        /// </summary>
        /// <param name="group"> The dictionary (drone ID --> Drone object) that the group will have. </param>
        public DroneGroup(Dictionary<string, Drone> group) : this(WorldProperties.getNextGroupId(), group) {}

        /// <summary>
        /// Disbands drones from the group with optional operations.
        /// Clears the group.
        /// </summary>
        /// <param name="removeGroupWaypoints"> If true, will delete group waypoints from the drone. </param>
        /// <param name="deselect"> If true, will remove the group from the current selection. </param>
        /// <param name="clearIDs"> If true, will set all drone's groupID fields to null </param>
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
            } else
            {
                foreach (Drone drone in dronesDict.Values)
                {
                    drone.AbsorbPreviousGroupWaypoints();
                }
                foreach (GroupWaypoint waypoint in waypoints)
                {
                    GameObject.Destroy(waypoint.gameObjectPointer);
                }
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

        /// <summary>
        /// Add a group waypoint at the specified position.
        /// </summary>
        /// <param name="position"> The position of the new waypoint to add. </param>
        /// <returns> The newly created GroupWaypoint object. </returns>
        public GroupWaypoint AddWaypoint(Vector3 position)
        {
            // Create the GroupWaypoint object and add it to waypoints.
            GroupWaypoint prevWaypoint = (waypoints.Count > 0) ? waypoints[waypoints.Count - 1] : null;
            GroupWaypoint waypoint = new GroupWaypoint(groupId, position, prevWaypoint);
            if (prevWaypoint != null)
            {
                prevWaypoint.SetNextWaypoint(waypoint);
            }
            waypoints.Add(waypoint);

            // Create and add corresponding Waypoint object to each of the drones.
            Dictionary<string, Waypoint> currWaypointDict = new Dictionary<string, Waypoint>();
            individualDroneWaypoints.Add(currWaypointDict);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                //Debug.Log("Creating waypoint for drone" + entry.Value.id);
                Waypoint newWaypoint = new Waypoint(entry.Value, waypoint.GetPosition());
                entry.Value.AddGroupWaypoint(newWaypoint, prevWaypoint == null);
                currWaypointDict.Add(entry.Value.id, newWaypoint);
            }

            return waypoint;
        }

        /// <summary>
        /// Insert a group waypoint at the specified position and before prevWaypoint.
        /// </summary>
        /// <param name="position"> The position of the new waypoint to add. </param>
        /// <param name="prevWaypoint"> The waypoint that the new waypoint should come after. </param>
        /// <returns> The newly created GroupWaypoint object. </returns>
        public GroupWaypoint InsertWaypoint(Vector3 position, GroupWaypoint prevWaypoint)
        {
            // Find the insert position.
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

            // Create and add the new GroupWaypoint object, updating previous waypoint pointers.
            GroupWaypoint waypoint = new GroupWaypoint(groupId, position, prevWaypoint);
            if (insertIndex < waypoints.Count)
            {
                waypoints[insertIndex].SetPrevWaypoint(waypoint);
                waypoint.SetNextWaypoint(waypoints[insertIndex]);
            }
            prevWaypoint.SetNextWaypoint(waypoint);
            waypoints.Insert(insertIndex, waypoint);

            // Perform waypoint insertion on each of the drones.
            Dictionary<string, Waypoint> prevWaypointDict = (insertIndex == 0) ? null : individualDroneWaypoints[insertIndex - 1];
            Dictionary<string, Waypoint> currWaypointDict = new Dictionary<string, Waypoint>();
            individualDroneWaypoints.Insert(insertIndex, currWaypointDict);
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                Waypoint dronePrevWaypoint = (prevWaypointDict == null) ? null : prevWaypointDict[entry.Key];
                Waypoint newWaypoint = new Waypoint(entry.Value, waypoint.GetPosition());
                entry.Value.InsertGroupWaypoint(newWaypoint, dronePrevWaypoint);
                currWaypointDict.Add(entry.Key, newWaypoint);
            }

            return waypoint;
        }

        /// <summary>
        /// Callback method to notify each of the drones of group waypoint modification.
        /// </summary>
        public void OnModifyWaypoint(GroupWaypoint modifiedWaypoint)
        {
            // Find the waypoint index.
            int waypoint_ind = findWaypoint(modifiedWaypoint);
            if (waypoint_ind < 0)
            {
                return;
            }

            // Call OnModifyWaypoint for each of the drones using its individual Waypoint object.
            Dictionary<string, Waypoint> droneWaypointDict = individualDroneWaypoints[waypoint_ind];
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.OnModifyWaypoint(droneWaypointDict[entry.Key]);
            }

            // Update UI.
            updateIndividualWaypointUI(waypoint_ind);
        }

        /// <summary>
        /// Update the UI of each of the drone's waypoint corresponding to the group waypoint.
        /// </summary>
        /// <param name="modifiedWaypoint"> The group waypoint that was modified.</param>
        public void UpdateIndividualWaypointUI(GroupWaypoint modifiedWaypoint)
        {
            int waypoint_ind = findWaypoint(modifiedWaypoint);
            if (waypoint_ind < 0)
            {
                return;
            }
            updateIndividualWaypointUI(waypoint_ind);
        }

        /// <summary>
        /// Helper method that actually does the waypoint UI updating.
        /// </summary>
        /// <param name="waypoint_ind"> The index of the GroupWaypoint that was updated. </param>
        private void updateIndividualWaypointUI(int waypoint_ind)
        {
            GroupWaypoint waypoint = waypoints[waypoint_ind];
            Dictionary<string, Waypoint> currWaypointDict = individualDroneWaypoints[waypoint_ind];
            foreach (Waypoint droneWaypoint in currWaypointDict.Values)
            {
                droneWaypoint.AssignPosition(waypoint.GetPosition());
            }
        }

        /// <summary>
        /// Delete group waypoint and remove it from all drones in group.
        /// </summary>
        /// <param name="waypoint"> The waypoint to delete. </param>
        /// <returns> True if group contains this waypoint and it can be deleted. False otherwise. </returns>
        public bool DeleteWaypoint(GroupWaypoint waypoint)
        {
            int waypoint_ind = findWaypoint(waypoint);
            if (waypoint_ind < 0)
            {
                return false;
            }
            deleteWaypointAtIndex(waypoint_ind);
            return true;
        }

        /// <summary>
        /// Delete all group waypoints.
        /// Removes them from the drones also.
        /// </summary>
        public void ClearWaypoints()
        {
            for (int i = waypoints.Count - 1; i >= 0; i--)
            {
                deleteWaypointAtIndex(i);
            }
        }

        /// <summary>
        /// Helper method to actually remove a group waypoint and
        /// deletes the corresponding Waypoints from drones in the group.
        /// </summary>
        /// <param name="waypoint_ind"> Index of the GroupWaypoint to delete. </param>
        private void deleteWaypointAtIndex(int waypoint_ind)
        {
            GroupWaypoint waypoint = waypoints[waypoint_ind];
            GeneralWaypoint prevWaypoint = waypoint.GetPrevWaypoint();
            GeneralWaypoint nextWaypoint = waypoint.GetNextWaypoint();
            Debug.Log("Deleting " + waypoint + " with prev " + prevWaypoint + " and next " + nextWaypoint);
            waypoint.SetPrevWaypoint(null);
            waypoint.SetNextWaypoint(null);
            if (prevWaypoint == null)
            {
                if (waypoint_ind < waypoints.Count - 1)
                {
                    Dictionary<string, Waypoint> currDroneWaypointDict = individualDroneWaypoints[waypoint_ind + 1];
                    foreach (Waypoint w in currDroneWaypointDict.Values)
                    {
                        w.SetVisible(true);
                    }
                }
                
            }
            else
            {
                prevWaypoint.SetNextWaypoint(nextWaypoint);
            }
            if (nextWaypoint != null)
            {
                nextWaypoint.SetPrevWaypoint(prevWaypoint);
            }

            // Deletes the waypoint from the individual drones.
            Dictionary<string, Waypoint> droneWaypointDict = individualDroneWaypoints[waypoint_ind];
            foreach (KeyValuePair<string, Drone> entry in dronesDict)
            {
                entry.Value.DeleteWaypoint(droneWaypointDict[entry.Key]);
            }

            // Remove waypoint from collections.
            waypoints.RemoveAt(waypoint_ind);
            individualDroneWaypoints.RemoveAt(waypoint_ind);
            // Remove waypoint from UI.
            waypoint.gameObjectPointer.GetComponent<WaypointProperties>().DeleteLineCollider();
            Object.Destroy(waypoint.gameObjectPointer);
        }

        /// <summary>
        /// Add group the the current selection. Will call OnSelect().
        /// </summary>
        public void Select()
        {
            WorldProperties.selectedDrones.AddGroup(this);
            OnSelect();
        }

        /// <summary>
        /// Update the UI of all drones in group to reflect selection.
        /// </summary>
        public void OnSelect()
        {
            foreach (Drone drone in this.dronesDict.Values)
            {
                drone.OnSelect();
            }
            this.selected = true;
        }

        /// <summary>
        /// Remove group the the current selection. Will call OnDeselect().
        /// </summary>
        public void Deselect()
        {
            WorldProperties.selectedDrones.DeselectGroup(this);
            OnDeselect();
        }

        /// <summary>
        /// Update the UI of all drones in group to reflect deselection.
        /// </summary>
        public void OnDeselect()
        {
            foreach (Drone drone in this.dronesDict.Values)
            {
                drone.OnDeselect();
            }
            this.selected = false;
        }

        /// <summary>
        /// Find index of the GroupWaypoint object in waypoints.
        /// </summary>
        /// <returns> The index of the waypoint in waypoints. </returns>
        private int findWaypoint(GroupWaypoint waypoint)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == waypoint)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get list of ID's of drones in the group.
        /// </summary>
        /// <returns> A list of all the ID's of the drones in the group. </returns>
        public List<string> getIDs()
        {
            return dronesDict.Keys.ToList();
        }

        /// <summary>
        /// Get list of Drone objects of drones in the group.
        /// </summary>
        /// <returns> The list of all the drone objects of the drones in the group.</Drone></returns>
        public List<Drone> getDrones()
        {
            return dronesDict.Values.ToList();
        }

        /// <summary>
        /// Get IEnumerable of Drone objects of drones in the group.
        /// </summary>
        /// <returns> an IEnumerable of the drone objects in the group. </returns>
        public IEnumerable<Drone> getDronesEnumerable()
        {
            return dronesDict.Values;
        }

        /// <summary>
        /// Get Drone object with a specific ID.
        /// </summary>
        /// <param name="id"> The ID of the target drone. </param>
        /// <returns> The drone with the ID. </returns>
        public Drone getDrone(string id)
        {
            return dronesDict[id];
        }

        /// <summary>
        /// Return dictionary of Drone.
        /// </summary>
        /// <returns> A Dictionary (drone ID --> Drone object) of all drones in the group. </returns>
        public Dictionary<string, Drone> getDronesDict()
        {
            return dronesDict;
        }
    }
}
