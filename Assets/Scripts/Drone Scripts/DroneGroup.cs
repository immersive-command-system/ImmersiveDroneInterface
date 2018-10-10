namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class DroneGroup
    {

        private Dictionary<string, Drone> dronesDict;
        public Color color;
        public string groupId;

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
