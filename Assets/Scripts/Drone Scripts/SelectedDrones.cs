namespace ISAACS
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class SelectedDrones
    {
        public Dictionary<string, Drone> individualDrones = new Dictionary<string, Drone>();
        public Dictionary<string, DroneGroup> groupedDrones = new Dictionary<string, DroneGroup>();
        public Color color;

        // since currently a drone is automatically spawned at the start
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
            } else
            {
                WorldProperties.groupedDrones[drone.groupID].Select();
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
            } else
            {
                WorldProperties.groupedDrones[drone.groupID].Deselect();
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
        ///Removes one collection of grouped drones to selection.
        /// </summary>
        /// <param name="groupID">Id of grouped drones to remove from selection</param>
        public void DeselectGroup(string groupID)
        {
            groupedDrones.Remove(groupID);
        }

        /// <summary>
        ///Removes one collection of grouped drones to selection.
        /// </summary>
        /// <param name="group">DroneGroup object to remove from selection</param>
        public void DeselectGroup(DroneGroup group)
        {
            DeselectGroup(group.groupId);
        }

        /// <summary>
        /// Deletes selected drones from map.
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
            individualDrones.Clear();
            groupedDrones.Clear();
        }

        /// <summary>
        /// Returns whether or not there are drones selected
        /// </summary>
        /// <returns>True if individual or grouped drone(s) are selected, False if no drones are selected</returns>
        public bool AreDronesSelected()
        {
            return individualDrones.Count > 0 || groupedDrones.Count > 0;
        }

        /// <summary>
        /// Returns whether or not only a single ungrouped drone is selected
        /// </summary>
        /// <returns>True if only one ungrouped drone is selected. False otherwise. </returns>
        public bool IsSingleDroneSelected()
        {
            return individualDrones.Count == 1 && groupedDrones.Count == 0;
        }

        /// <summary>
        /// Returns whether or not only a single drone group is selected
        /// </summary>
        /// <returns>True if only one drone group is selected. False otherwise. </returns>
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
        public GeneralWaypoint AddWayPoint(Vector3 waypointPosition)
        {
            if (!AreDronesSelected())
            {
                return null;
            }
            if (IsSingleDroneSelected()) {
                Drone drone = individualDrones.Values.Single();
                Waypoint newWaypoint = new Waypoint(drone, waypointPosition);
                drone.AddWaypoint(newWaypoint);
                return newWaypoint;
            } else
            {
                DroneGroup singleGroup = null;
                if (IsSingleGroupSelected())
                {
                    singleGroup = groupedDrones.Values.Single();
                } else
                {
                    singleGroup = CreateNewGroup();
                }
                return singleGroup.AddWaypoint(waypointPosition);
            }

        }

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
        /// <param name="waypoint">GeneralWaypoint object to delete from selected drones and groups.</param>
        public void DeleteWayPoint(GeneralWaypoint waypoint) //should we keep track of old groups in case the user decides to not add any waypoints for the newly selected-drones group?
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
        /// <returns>Newly created DroneGroup object from the current select, if valid. Returns null otherwise.</returns>
        private DroneGroup CreateNewGroup()
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
