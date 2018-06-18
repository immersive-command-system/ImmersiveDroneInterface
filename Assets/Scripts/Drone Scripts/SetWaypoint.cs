namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;
    using ROSBridgeLib;
    using ROSBridgeLib.interface_msgs;
    using SimpleJSON;

    public class SetWaypoint : MonoBehaviour {

        public Drone thisDrone; // Drone classObject

        public GameObject waypoint; // Waypoint object
        public static GameObject controller_right;

        public float maxHeight; // maximum height waypoint can be at when adjusting
        public bool toggleDeselectOtherDrones;

        private GameObject controller; // Pointer Controller
        private GameObject world; // Refers to the ground
        private Vector3 groundPoint; // Vector3 indicating where the pointer is pointing on the ground
        private bool firstClickFinished = false;
        private static bool adjustingHeight = false;
        
        private Vector3 currentScale; // Current Scale of the World
        private Vector3 originalScale; // Scale that the world started in
        public Vector3 actualScale; // currentScale / OriginalScale
        private static bool clearWaypointsToggle;

        public bool settingIntermediateWaypoint;
        private static bool currentlySetting = false;
        public GameObject LineOriginWaypoint;

        
        private static bool setWaypointState = false;

        // SECONDARY PLACEMENT METHOD

        // UTILITY FUNCTIONS


    }
}
