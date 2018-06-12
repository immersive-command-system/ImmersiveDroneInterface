namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class WorldProperties : MonoBehaviour
    {

        public static Dictionary<char, Drone> dronesDict;
        public static Drone selectedDrone;
        public static char nextDroneId;
        
        // Use this for initialization
        void Start()
        {
            dronesDict = new Dictionary<char, Drone>(); // Collection of all the drone classObjects
            nextDroneId = 'A'; // Used as an incrementing key for the dronesDict and for a piece of the communication about waypoints across the ROSBridge
        }
    }
}
