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
        private GameObject adjustingWaypoint; // new instantiated waypoint
        private Vector3 currentScale; // Current Scale of the World
        private Vector3 originalScale; // Scale that the world started in
        public Vector3 actualScale; // currentScale / OriginalScale
        private static bool clearWaypointsToggle;

        public bool settingIntermediateWaypoint;
        private static bool currentlySetting = false;
        public GameObject LineOriginWaypoint;

        private static GameObject placePoint; // Place waypoint in front of controller
        private GameObject currentWaypoint; // The current waypoint we are trying to place
        private bool placeAtHand = false; // Are we placing the waypoint at our hand?
        public Material placePointMaterial;
        public Material adjustMaterial;
        private static bool setWaypointState = false;

        void Start()
        {
            thisDrone = gameObject.GetComponent<DroneProperties>().classPointer;

            controller_right = GameObject.Find("controller_right");
            adjustingHeight = false;
            world = GameObject.FindGameObjectWithTag("World");
            controller = GameObject.FindGameObjectWithTag("GameController");
            settingIntermediateWaypoint = false;
            placePoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            placePoint.transform.parent = controller.GetComponent<VRTK_ControllerEvents>().transform;
            placePoint.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            placePoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            placePoint.SetActive(true);
        }

        
        void Update()
        {
           
        }

        // SECONDARY PLACEMENT METHOD

        // Allows user to select the point on the terrain that the waypoint will appear above.
        private GameObject SetGroundpoint()

        {
            if (ControllerInteractions.IsRaycastOn())
            {

                if (controller.GetComponent<VRTK_StraightPointerRenderer>().OnGround())
                {
                    adjustingHeight = true;
                    groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetGroundPoint();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                groundPoint = placePoint.transform.position;
                placeAtHand = true;
            }
            GameObject newWaypoint = CreateWaypoint(groundPoint);
            currentWaypoint = newWaypoint;
            return newWaypoint;
        }

        // Adjusts the height at which the waypoint appears
        private void AdjustHeight(GameObject newWaypoint)
        {
            float groundX = newWaypoint.transform.position.x;
            float groundY = newWaypoint.transform.position.y;
            float groundZ = newWaypoint.transform.position.z;
            float localX = controller.transform.position.x;
            float localY = controller.transform.position.y;
            float localZ = controller.transform.position.z;
            float height = 2.147f + (float)Distance(groundX, groundZ, 0f, 0f, localX, localZ) * (float)Math.Tan(Math.PI * (ControllerInteractions.getLocalControllerRotation(OVRInput.Controller.RTouch).x));
            float heightMin = 2.3f + actualScale.y / 200; //mesh height = 2.147

            height = Math.Min(MaxHeight(), Math.Max(heightMin, height));
            newWaypoint.transform.position = new Vector3(groundX, height, groundZ);

            adjustingHeight = !ControllerInteractions.secondIndexPressed();
            firstClickFinished = !ControllerInteractions.secondIndexPressed();
            settingIntermediateWaypoint = !ControllerInteractions.secondIndexPressed();
            currentlySetting = !ControllerInteractions.secondIndexPressed();
            if (!adjustingHeight)
            {
                activatePlacePoint();
            }

        }

        // UTILITY FUNCTIONS

        private double Distance(float groundX, float groundZ, float groundY, float controllerY, float controllerX, float controllerZ)
        {
            return Math.Sqrt(Math.Pow((controllerX - groundX), 2) + Math.Pow((controllerY - groundY),2)+ Math.Pow((controllerZ - groundZ), 2));
        }

        //Activate PlacePoint
        private static void activatePlacePoint()
        {
            placePoint.SetActive(true);
        }

        //Deactivate PlacePoint
        private static void deactivatePlacePoint()
        {
            placePoint.SetActive(false);
        }

    }
}
