/* 
 *
 * 
 */

namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SelectedDrones
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
        /// Adds an individual drone to selection
        /// </summary>
        /// <param name="drone">Drone object to add</param>
        public void addDrone(Drone drone)
        {
            individualDrones[drone.id] = drone;
        }

        /// <summary>
        /// Adds an individual drone to selection
        /// </summary>
        /// <param name="droneID">Id of drone object to add</param>
        public void addDrone(string droneID)
        {
            individualDrones[droneID] = WorldProperties.dronesDict[droneID];
        }

        /// <summary>
        /// Adds one collection of grouped drones to selection
        /// </summary>
        /// <param name="group">Id of grouped drones to add</param>
        public void addGroup(string groupID)
        {
            groupedDrones[groupID] = WorldProperties.groupedDrones[groupID];
        }

        /// <summary>
        /// Deletes selected drones from map
        /// </summary>
        public void delete()
        {
            //should this be a feature?
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
        public bool dronesAreSelected()
        {
            return individualDrones.Count != 0 || groupedDrones.Count != 0;
        }

        public void addWayPoint(Waypoint newWaypoint)
        {
            createNewGroup();
            ungroupOldGroups();
        }

        public void insertWayPoint(Waypoint newWaypoint)
        {

        }

        public void deleteWayPoint(Waypoint deletedWaypoint)
        {

        }

        /// <summary>
        /// A new group is created if more than one drone/drone groups are selected and waypoint(s) are added for the drones.
        /// </summary>
        private void createNewGroup()
        {
            Dictionary<string, Drone> newGroup = new Dictionary<string, Drone>();
            string groupIdName = WorldProperties.getNextGroupId();

            foreach(KeyValuePair<string, Drone> entry in individualDrones)
            {
                newGroup[entry.Key] = entry.Value;
            }
            
            foreach(DroneGroup group in groupedDrones.Values)
            {
                foreach(KeyValuePair<string, Drone> entry in group.getDronesDict())
                {
                    newGroup[entry.Key] = entry.Value;
                }
            }

            WorldProperties.groupedDrones[WorldProperties.getNextGroupId()] = new DroneGroup(newGroup);
            
        }


        /// <summary>
        /// Ungroups the previously grouped drones that make up the selection if more than one drone/drone groups are selected and waypoint(s) are added for the drones.
        /// </summary>
        private void ungroupOldGroups()
        {
            foreach (string groupID in groupedDrones.Keys)
            {
                WorldProperties.groupedDrones.Remove(groupID);
            }

        }

    }
}
