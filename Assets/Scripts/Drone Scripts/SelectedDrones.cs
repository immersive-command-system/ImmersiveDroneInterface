namespace ISAACS
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This class is a container representing a selection of drones in the interface.
    /// </summary>
    public class SelectedDrones
    {
        // Drones in the selection that aren't part of a group.
        public Dictionary<string, Drone> individualDrones = new Dictionary<string, Drone>();
        // Drone groups in the selection.
        public Dictionary<string, DroneGroup> groupedDrones = new Dictionary<string, DroneGroup>();
        // The color of the selection.
        public Color color;
        
        public SelectedDrones()
        {
            color = WorldProperties.droneSelectionColor;
        }

        /// <summary>
        /// Adds an individual drone to selection.
        /// If drone is part of a group, adds entire group to the selection.
        /// </summary>
        /// <param name="drone">Drone object to add</param>
        public void AddDrone(Drone drone)
        {
            if (drone.groupID == null)
            {
                individualDrones[drone.id] = drone;
				WorldProperties.selectedDroneMenu.UpdateMenu_Select_Drone (drone.id);
            } else
            {
                WorldProperties.groupedDrones[drone.groupID].Select();
				WorldProperties.selectedDroneMenu.UpdateMenu_Select_Group (drone.groupID);
            }

        }

        /// <summary>
        /// Adds an individual drone to selection.
        /// If drone is part of a group, adds entire group to the selection.
        /// </summary>
        /// <param name="droneID">Id of drone object to add</param>
        public void AddDrone(string droneID)
        {
            AddDrone(WorldProperties.dronesDict[droneID]);
        }

        /// <summary>
        /// Remove drone from selection.
        /// If drone is part of a group, removes entire group from the selection.
        /// </summary>
        /// <param name="drone">Drone object to deselect</param>
        public void DeselectDrone(Drone drone)
        {
            if (drone.groupID == null)
            {
                individualDrones.Remove(drone.id);
				WorldProperties.selectedDroneMenu.UpdateMenu_Deselect_Drone (drone.id);

            } else
            {
                WorldProperties.groupedDrones[drone.groupID].Deselect();
				WorldProperties.selectedDroneMenu.UpdateMenu_Deselect_Group (drone.groupID);
            }

        }

        /// <summary>
        /// Remove drone from selection.
        /// If drone is part of a group, removes entire group from the selection.
        /// </summary>
        /// <param name="droneID">Id of drone object to deselect</param>
        public void DeselectDrone(string droneID)
        {
            DeselectDrone(WorldProperties.dronesDict[droneID]);
        }

        /// <summary>
        /// Adds one collection of grouped drones to selection.
        /// </summary>
        /// <param name="groupID">Id of grouped drones to add</param>
        public void AddGroup(string groupID)
        {
            AddGroup(WorldProperties.groupedDrones[groupID]);
        }

        /// <summary>
        /// Adds one collection of grouped drones to selection.
        /// </summary>
        /// <param name="group">DroneGroup object to add</param>
        public void AddGroup(DroneGroup group)
        {
            groupedDrones[group.groupId] = group;
        }

        /// <summary>
        /// Removes one collection of grouped drones to selection.
        /// </summary>
        /// <param name="groupID">Id of grouped drones to remove from selection</param>
        public void DeselectGroup(string groupID)
        {
            groupedDrones.Remove(groupID);
        }

        /// <summary>
        /// Removes one collection of grouped drones to selection.
        /// </summary>
        /// <param name="group">DroneGroup object to remove from selection</param>
        public void DeselectGroup(DroneGroup group)
        {
            DeselectGroup(group.groupId);
        }

        /// <summary>
        /// Deletes selected drones from map. NOT IMPLEMENTED YET.
        /// </summary>
        public void deleteDrones()
        {
            //should this be a feature?
            //how sure should we be that the user actually wants to do this before continuing?
        }

        /// <summary>
        /// Clears selections, i.e. nothing should be selected after running this method
        /// </summary>
        public void Clear()
        {
            foreach (Drone drone in individualDrones.Values)
            {
                drone.OnDeselect();
            }
            individualDrones.Clear();
            foreach (DroneGroup group in groupedDrones.Values)
            {
                group.OnDeselect();
            }
            groupedDrones.Clear();
        }

        /// <summary>
        /// Returns whether or not there are drones selected
        /// </summary>
        /// <returns> True if something is selected. False otherwise </returns>
        public bool AreDronesSelected()
        {
            return individualDrones.Count > 0 || groupedDrones.Count > 0;
        }

        /// <summary>
        /// Returns whether or not only a single ungrouped drone is selected
        /// </summary>
        /// <returns> True if only one ungrouped drone is selected. False otherwise. </returns>
        public bool IsSingleDroneSelected()
        {
            return individualDrones.Count == 1 && groupedDrones.Count == 0;
        }

        /// <summary>
        /// Returns whether or not only a single drone group is selected
        /// </summary>
        /// <returns> True if only one drone group is selected. False otherwise. </returns>
        public bool IsSingleGroupSelected()
        {
            return individualDrones.Count == 0 && groupedDrones.Count == 1;
        }

        /// <summary>
        /// Add a waypoint to the selection.
        /// If the selection is not an individual ungrouped drone or a single group,
        /// the selection will first be regrouped into one DroneGroup.
        /// </summary>
        /// <param name="waypointPosition">Position of new waypoint.</param>
        /// <returns>
        /// Newly created waypoint.
        /// If only one individual drone is selected, then the returned waypoint is a Waypoint object.
        /// Otherwise, a GroupWaypoint object is returned.
        /// </returns>
        public GeneralWaypoint AddWayPoint(Vector3 waypointPosition)
        {
            // Do nothing if nothing is selected.
            if (!AreDronesSelected())
            {
                return null;
            }

            // Do single drone waypoint insertion if only one ungrouped drone is selected.
            if (IsSingleDroneSelected()) {
                Drone drone = individualDrones.Values.Single();
                Waypoint newWaypoint = new Waypoint(drone, waypointPosition);
                drone.AddWaypoint(newWaypoint);
                return newWaypoint;
            } else
            {
                DroneGroup singleGroup = null;
                // Check if only a single group is selected.
                // If not then regroup all drones in selection to a single group.
                if (IsSingleGroupSelected())
                {
                    singleGroup = groupedDrones.Values.Single();
                } else
                {
                    //should we keep track of old groups in case the user decides to not add any waypoints for the newly selected-drones group?
                    singleGroup = CreateNewGroup();
                }
                
                return singleGroup.AddWaypoint(waypointPosition);
            }

        }

        /// <summary>
        /// Insert waypoint after prevWaypoint.
        /// This function is only valid if a single drone or a single group is selected.
        /// </summary>
        /// <param name="position"> Position of new waypoint. </param>
        /// <param name="prevWaypoint"> Waypoint after which to insert the new waypoint. </param>
        /// <returns>
        /// Newly created waypoint.
        /// If only one individual drone is selected, then the returned waypoint is a Waypoint object.
        /// Otherwise, a GroupWaypoint object is returned.
        /// </returns>
        public GeneralWaypoint InsertWayPoint(Vector3 position, GeneralWaypoint prevWaypoint)
        {
            if (IsSingleDroneSelected())
            {
                Drone currDrone = individualDrones.Values.Single();
                Waypoint newWaypoint = new Waypoint(currDrone, position);
                currDrone.InsertWaypoint(newWaypoint, (Waypoint)prevWaypoint);
                return newWaypoint;
            }
            else if (IsSingleGroupSelected())
            {
                return groupedDrones.Values.Single().InsertWaypoint(position, (GroupWaypoint)prevWaypoint);
            }
            return null;
        }

        /// <summary>
        /// Deletes waypoint from selection.
        /// Only works if selection is a single drone or single DroneGroup.
        /// </summary>
        /// <param name="waypoint"> GeneralWaypoint object to delete from selected drones and groups. </param>
        public void DeleteWayPoint(GeneralWaypoint waypoint)
        {
            if (IsSingleDroneSelected())
            {
                individualDrones.Values.Single().DeleteWaypoint((Waypoint)waypoint);
            }
            else if (IsSingleGroupSelected())
            {
                DroneGroup singleGroup = groupedDrones.Values.Single();
                singleGroup.DeleteWaypoint((GroupWaypoint)waypoint);
            }
        }

        /// <summary>
        /// A new group is created if more than one drone/drone groups are selected and waypoint(s) are added for the drones.
        /// Creating a group is valid if more than one individual drone or more than one group or 1 drone and 1 group is selected.
        /// </summary>
        /// <returns> Newly created DroneGroup object from the current select, if valid. Returns null otherwise. </returns>
        public DroneGroup CreateNewGroup()
        {
            if (IsSingleDroneSelected() || IsSingleGroupSelected() || !AreDronesSelected())
            {
                return null;
            }
            Dictionary<string, Drone> drones = new Dictionary<string, Drone>();
            string groupIdName = WorldProperties.getNextGroupId();

            foreach(KeyValuePair<string, Drone> entry in individualDrones)
            {
                entry.Value.groupID = groupIdName;
                drones[entry.Key] = entry.Value;
            }
           
            foreach(DroneGroup group in groupedDrones.Values)
            {
                foreach (KeyValuePair<string, Drone> entry in group.getDronesDict())
                {
                    entry.Value.groupID = groupIdName;
                    drones[entry.Key] = entry.Value;
                }
                group.Ungroup(false, false, false);
            }

            DroneGroup newGroup = new DroneGroup(groupIdName, drones);

            individualDrones.Clear();
            groupedDrones.Clear();
            groupedDrones[newGroup.groupId] = newGroup;

            WorldProperties.groupedDrones[groupIdName] = newGroup;

            return newGroup;
        }

        /// <summary>
        /// Gets a flattened list of all the drones in the selection (both grouped and ungrouped).
        /// </summary>
        /// <returns> A list of all the drones in the selection (both grouped and ungrouped). </returns>
        public List<Drone> GetFlattenedDroneList()
        {
            List<Drone> droneList = new List<Drone>();
            droneList.AddRange(individualDrones.Values);
            foreach (DroneGroup group in groupedDrones.Values)
            {
                // Want to call the enumerable version of getDrones so
                // less overhead of materializing it into a list if it'll only
                // be used to add it to this droneList
                droneList.AddRange(group.getDronesEnumerable());
            }
            return droneList;
        }

        private void printIndividualDroneIDs()
        {
            string idList = "";
            foreach (string id in individualDrones.Keys)
            {
                idList += id + "\n";
            }
            Debug.Log("Individual Drones:\n" + idList);
        }
    }
}
