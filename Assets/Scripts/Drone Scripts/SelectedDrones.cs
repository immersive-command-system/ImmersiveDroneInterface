/* 
 *
 * 
 */

namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class SelectedDrones //should SelectedDrones be a DroneGroup???
    {
        public Dictionary<string, Drone> individualDrones;
        public Dictionary<string, DroneGroup> groupedDrones;
        public Color color;

        // since currently a drone is automatically spawned at the start
        public SelectedDrones(Drone initialDrone)
        {
            individualDrones[initialDrone.id] = initialDrone;
            color = WorldProperties.droneSelectionColor;
        }

        /// <summary>
        /// Adds an individual drone to selection.
        /// If drone is part of a group, adds entire group to the selection.
        /// </summary>
        /// <param name="drone">Drone object to add</param>
        public void addDrone(Drone drone)
        {
            if (drone.groupID == null)
            {
                individualDrones[drone.id] = drone;
            } else
            {
                addGroup(drone.groupID);
            }
            
        }

        /// <summary>
        /// Adds an individual drone to selection.
        /// If drone is part of a group, adds entire group to the selection.
        /// </summary>
        /// <param name="droneID">Id of drone object to add</param>
        public void addDrone(string droneID)
        {
            addDrone(WorldProperties.dronesDict[droneID]);
        }

        /// <summary>
        /// Remove drone from selection.
        /// If drone is part of a group, removes entire group from the selection.
        /// </summary>
        /// <param name="droneID">Drone object to deselect</param>
        public void deselectDrone(Drone drone)
        {
            if (drone.groupID == null)
            {
                individualDrones.Remove(drone.id);
            } else
            {
                deselectGroup(drone.groupID);
            }
        }

        /// <summary>
        /// Remove drone from selection.
        /// If drone is part of a group, removes entire group from the selection.
        /// </summary>
        /// <param name="droneID">Id of drone object to deselect</param>
        public void deselectDrone(string droneID)
        {
            deselectDrone(WorldProperties.dronesDict[droneID]);
        }

        /// <summary>
        /// Adds one collection of grouped drones to selection.
        /// </summary>
        /// <param name="group">Id of grouped drones to add</param>
        public void addGroup(string groupID)
        {
            groupedDrones[groupID] = WorldProperties.groupedDrones[groupID];
        }

        /// <summary>
        ///Removes one collection of grouped drones to selection.
        /// </summary>
        /// <param name="group">Id of grouped drones to add</param>
        public void deselectGroup(string groupID)
        {
            groupedDrones.Remove(groupID);
        }

        /// <summary>
        /// Deletes selected drones from map.
        /// </summary>
        public void deleteDrones()
        {
            //should this be a feature?
            //how sure should we be that the user actually wants to do this?
        }

        /// <summary>
        /// Clears selections, i.e. nothing should be selected after running this method
        /// </summary>
        public void clear()
        {
            individualDrones.Clear();
            groupedDrones.Clear();
        }

        /// <summary>
        /// Returns whether or not there are drones selected
        /// </summary>
        /// <returns>True if individual or grouped drone(s) are selected, False if no drones are selected</returns>
        public bool areDronesSelected()
        {
            return individualDrones.Count != 0 || groupedDrones.Count != 0;
        }

        /// <summary>
        /// Returns whether or not only a single ungrouped drone is selected
        /// </summary>
        /// <returns>True if only one ungrouped drone is selected. False otherwise. </returns>
        public bool isSingleDroneSelected()
        {
            return individualDrones.Count == 1 && groupedDrones.Count == 0;
        }

        /// <summary>
        /// Returns whether or not only a single drone group is selected
        /// </summary>
        /// <returns>True if only one drone group is selected. False otherwise. </returns>
        public bool isSingleGroupSelected()
        {
            return individualDrones.Count == 0 && groupedDrones.Count == 0;
        }

        /// <summary>
        /// Add a waypoint to the selection.
        /// If the selection is not an individual ungrouped drone or a single group,
        /// the selection will first be regrouped into one DroneGroup.
        /// </summary>
        public void addWayPoint(Waypoint newWaypoint)
        {
            if (isSingleDroneSelected()) {
                individualDrones.Values.Single().AddWaypoint(newWaypoint);
            } else
            {
                DroneGroup singleGroup = null;
                if (isSingleGroupSelected())
                {
                    singleGroup = groupedDrones.Values.Single();
                } else
                {
                    singleGroup = createNewGroup();
                }
                singleGroup.AddWaypoint(newWaypoint);
            }

        }

        public void insertWayPoint(Waypoint newWaypoint, Waypoint prevWaypoint)
        {
            if (isSingleDroneSelected())
            {
                individualDrones.Values.Single().InsertWaypoint(newWaypoint, prevWaypoint);
            }
            else
            {
                // Unsure of what to do here?
            }
        }

        /// <summary>
        /// Deletes waypoint from selection.
        /// Only works if selection is a single drone or single DroneGroup.
        /// </summary>
        public void deleteWayPoint(Waypoint deletedWaypoint) //should we keep track of old groups in case the user decides to not add any waypoints for the newly selected-drones group?
        {
            if (isSingleDroneSelected())
            {
                individualDrones.Values.Single().DeleteWaypoint(deletedWaypoint);
            } else if (isSingleGroupSelected())
            {
                DroneGroup singleGroup = groupedDrones.Values.Single();
                singleGroup.DeleteWaypoint(deletedWaypoint);
            }
        }

        /// <summary>
        /// A new group is created if more than one drone/drone groups are selected and waypoint(s) are added for the drones.
        /// Creating a group is valid if more than one individual drone or more than one group or 1 drone and 1 group is selected.
        /// </summary>
        /// <returns>Newly created DroneGroup object from the current select, if valid. Returns null otherwise.</returns>
        private DroneGroup createNewGroup()
        {
            if (isSingleDroneSelected() || isSingleGroupSelected() || !areDronesSelected())
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
                WorldProperties.groupedDrones.Remove(group.groupId);
                foreach (KeyValuePair<string, Drone> entry in group.getDronesDict())
                {
                    entry.Value.groupID = groupIdName;
                    drones[entry.Key] = entry.Value;
                }
            }

            DroneGroup newGroup = new DroneGroup(groupIdName, drones);

            individualDrones.Clear();
            groupedDrones.Clear();
            groupedDrones[newGroup.groupId] = newGroup;

            WorldProperties.groupedDrones[groupIdName] = newGroup;

            return newGroup;
        }
    }
}
