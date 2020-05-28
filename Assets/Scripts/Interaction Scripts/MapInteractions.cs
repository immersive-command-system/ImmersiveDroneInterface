namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Mapbox.Unity.Map;
    using Mapbox.Utils;

    public class MapInteractions : MonoBehaviour
    {
        //terrain is the textured heightmap
        public GameObject[] terrain;
        //pivot is the center of the table
        public GameObject pivot;
        //originalScale is the original localScale of the world
        public Vector3 originalScale;
        //actualScale is the relative localScale of the world in comparison to its original localScale
        public Vector3 actualScale;
        //currentScale is the current localScale
        public Vector3 currentScale;
        //This is the original position of the world
        public Vector3 originalPosition;
        //This is the 1/10th of the originalScale of the world
        public Vector3 minScale;
        //This is the 10 times the originalScale of the world
        public Vector3 maxScale;
        //This is the speed at which the map can be moved at
        public float speed = 3;
        // Rotation speed (in rev/s)
        public float rotSpeed = 1;
        //This is the rotating flag
        public bool isRotating;
        //This tells us if the map is still moving or being dragged
        public enum MapState
        {
            IDLE, DRAGGING, MOVING, ROTATING, SCALING
        }
        // The radius of the table (assuming the table is circular)
        public float tableRadius;
        // The radius of the map (assuming the map is circular)
        public float mapRadius;
        // The circular table
        public GameObject rotatingTable;

        // Rotation stuff
        public LinkedList<float> angles;
        public bool handleHeldTrigger = false;
        public static MapState mapState;
        public OVRInput.Controller currentController;
        private Vector3 oldVec;
        private float movementAngle;
        public float movementAngleDecay = .95f;

        // Pointer Controller
        private GameObject controller;
        private VRTK.VRTK_StraightPointerRenderer pointer;

        // Peru: 3/7/2020 : Map Integration Variables
        public bool droneOutOfBounds = false;


        public bool citySimActive = false;
        public GameObject citySimParent;
        public GameObject citySim;
        public AbstractMap abstractMap;

        public float initZoom_citySim = 21.0f;
        public double initLat_citySim;
        public double initLong_citySim;
        public Vector3 initPosition_citySim;

        public double currLat_citySim;
        public double currLong_citySim;
        public Vector3 currPosition_citySim;

        public float minZoom_citySim = 0.0f;
        public float maxZoom_citySim = 22.0f;


        // Use this for initialization
        void Start()
        {
            //Terrain assignment
            terrain = GameObject.FindGameObjectsWithTag("Ground");

            //Pivot assignment
            pivot = GameObject.FindWithTag("Table");

            //This provides us with basis to create bounds on scaling and something to return to
            originalScale = transform.localScale;
            originalPosition = transform.position;
            actualScale = new Vector3(1, 1, 1);

            //These are the bounds on scaling
            minScale = Vector3.Scale(originalScale, new Vector3(0.1F, 0.1F, 0.1F));
            maxScale = Vector3.Scale(originalScale, new Vector3(10F, 10F, 10F));

            //handle rotation
            mapState = MapState.IDLE;
            angles = new LinkedList<float>();

            //For accessing StraightPointerRenderer and gradually phase it out
            controller = GameObject.FindGameObjectWithTag("GameController");
            pointer = controller.GetComponent<VRTK.VRTK_StraightPointerRenderer>();
        }

        void FixedUpdate()
        {

            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                // SCALE WORLD - if both grip triggers are held
                ScaleWorld();
                UpdateScale();
            }
            else
            {
                //POINTER MODE - this method should check for just one grip input and activate the pointer mode.

                // ROTATE WORLD - these methods check for just one grip input on a turntable handle or the right joystick moving
                ControllerRotateWorld();
                //ManualRotateWorld();
            }

            // MOVING WORLD
            MoveWorld();
            EnforceMapBoundary();
        }

        // Rotate the world based off of the right thumbstick
        private void ControllerRotateWorld()
        {
            float deltaX = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;

            // We only consider inputs above a certain threshold.
            if (Mathf.Abs(deltaX) > 0.2f)
            {
                mapState = MapState.IDLE; // Controller input overrides manual
                float angle = deltaX * rotSpeed * 360 * Time.fixedDeltaTime;
                transform.RotateAround(pivot.transform.position, Vector3.up, angle);

                // Peru: 3/7/2020 : Map Integration Rotate
                if (citySimActive)
                {
                    citySimParent.transform.RotateAround(pivot.transform.position, Vector3.up, angle);
                }

                // Peru: 5/28/2020 : Point Cloud Rotate
                GameObject pointCloud = GameObject.Find("PointCloud");

                if (pointCloud)
                {
                    pointCloud.transform.RotateAround(pivot.transform.position, Vector3.up, angle);
                }

                if (rotatingTable)
                {
                    rotatingTable.transform.RotateAround(pivot.transform.position, Vector3.up, angle);
                }
                mapState = MapState.ROTATING;
            }
            else if (mapState == MapState.ROTATING)
            {
                mapState = MapState.IDLE;
            }
        }

        // Rotate the world based off of physical movement/interaction
        private void ManualRotateWorld()
        {
            // CASE: User has held the handle.
            if (handleHeldTrigger)
            {
                // Update state.
                handleHeldTrigger = false;
                mapState = MapState.DRAGGING;

                // Initialize oldVec: direction vec from hand to pivot.
                oldVec = OVRInput.GetLocalControllerPosition(currentController) - pivot.transform.position;
                oldVec.y = 0;
                oldVec = Vector3.Normalize(oldVec);

                // Initialize angles: linked list that'll contain recent angle rotations.
                angles.Clear();
            }

            // CASE: Map is in the dragging state.
            if (mapState == MapState.DRAGGING)
            {
                // CASE: User has released the handle.
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, currentController) < .1f)
                {
                    // CASE: User was dragging the table when they released the handle.
                    if (mapState == MapState.DRAGGING)
                    {
                        // Initize movementAngle: the initial movement per fixedupdate, to avg of recent rots
                        movementAngle = 0;
                        foreach (float a in angles)
                            movementAngle += a;
                        movementAngle /= angles.Count;

                        // Set the map to moving.
                        mapState = MapState.MOVING;
                    }
                }

                // Initialize currVec.
                Vector3 currVec = OVRInput.GetLocalControllerPosition(currentController) - pivot.transform.position;
                currVec.y = 0;
                currVec = Vector3.Normalize(currVec);

                // Find the angle going from oldVec to currVec.
                float angle = Vector3.Angle(oldVec, currVec);
                Vector3 cross = Vector3.Cross(oldVec, currVec);
                if (cross.y > 0) angle = -angle;

                // Rotate the map and circular table by that angle.
                transform.RotateAround(pivot.transform.position, Vector3.up, angle);
                rotatingTable.transform.Rotate(Vector3.up, angle);

                // Update startVec
                oldVec = currVec;

                // Add recent rotation angle to angles. Keep only the N most recent angles.
                angles.AddLast(angle);
                if (angles.Count > 10)
                    angles.RemoveFirst();

            }
            else if (mapState == MapState.MOVING)
            {
                // Rotate the map/table by the movementAngle.
                transform.RotateAround(pivot.transform.position, Vector3.up, movementAngle);
                rotatingTable.transform.Rotate(Vector3.up, movementAngle);

                // Decay movementAngle.
                movementAngle *= movementAngleDecay;

                // Check if we've stopped moving - if so, set to IDLE.
                if (Mathf.Abs(movementAngle) < 0.005f)
                {
                    mapState = MapState.IDLE;
                }
            }
        }

        private void MoveWorld()
        {
            float moveX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
            float moveZ = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
            
            if (moveX != 0 || moveZ != 0)
            {
                // update map position based on input
                Vector3 position = transform.position;

                position.x += moveX * speed * Time.deltaTime;
                position.z += moveZ * speed * Time.deltaTime;

                transform.position = position;
                mapState = MapState.MOVING;

                // Peru: 3/7/2020 : Map Integration Move
                // TODO: Fix movement if it's along the circumference of the table
                if (citySimActive && !droneOutOfBounds)
                {
                    // TODO: find relation between speed and spped_cityMap 
                    // (it'll relate to scale somehow)
                    float speed_cityMap = speed *2;

                    // Calculate delta X,Z
                    float deltaX = moveX * speed_cityMap * Time.deltaTime;
                    float deltaZ = moveZ * speed_cityMap * Time.deltaTime;

                    // Update unity currPosition
                    // TODO: make local based on roatation
                    currPosition_citySim.x -= deltaX;
                    currPosition_citySim.z += deltaZ;

                    // Find new Lat,Long
                    // TODO: Well shit
                    double newLat_citySim = WorldProperties.UnityXToLat(initLat_citySim, currPosition_citySim.x) ;
                    double newLong_citySim = WorldProperties.UnityZToLong(initLong_citySim, initLat_citySim, currPosition_citySim.z) ;

                    // Update current position
                    currLat_citySim = newLat_citySim;
                    currLong_citySim = newLong_citySim;
                    
                    Vector2d finalLatLong = new Vector2d(currLat_citySim, currLong_citySim);
                    abstractMap.UpdateMap(finalLatLong);
                }

            }
            else if (mapState == MapState.MOVING)
            {
                mapState = MapState.IDLE;
            }

           
        }


        private void ScaleWorld()
        {
            //Obtaining distance and velocity
            Vector3 d = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) - OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Vector3 v = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch) - OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

            //Calculating Scaling Vector
            float result = Vector3.Dot(v, d);

            //Adjusting result to slow scaling
            float final_result = 1.0F + 0.2F * result;

            Vector3 scalingFactor = Vector3.Scale(transform.localScale, new Vector3(final_result, final_result, final_result));

            //Checking Scaling Bounds
            if (scalingFactor.sqrMagnitude > minScale.sqrMagnitude && scalingFactor.sqrMagnitude < maxScale.sqrMagnitude)
            {
                Vector3 A = transform.position;
                Vector3 B = pivot.transform.position;
                B.y = A.y;

                Vector3 startScale = transform.localScale;
                Vector3 endScale = transform.localScale * final_result;

                Vector3 C = A - B; // diff from object pivot to desired pivot/origin

                // calc final position post-scale
                Vector3 FinalPosition = (C * final_result) + B;

                // finally, actually perform the scale/translation
                transform.localScale = endScale;
                transform.position = FinalPosition;
                mapState = MapState.SCALING;

                // Peru: 3/7/2020 : Map Integration Scale
                if (citySimActive)
                {
                    // Update Zoom of the cityMap
                    //TODO: Figure out the scaling
                    float currZoom = abstractMap.Zoom;
                    float zoomScale = final_result; 

                    abstractMap.UpdateMap(currZoom* zoomScale);
                }
            }
            else if (mapState == MapState.SCALING)
            {
                mapState = MapState.IDLE;
            }
        }

        private void UpdateScale()
        {
            currentScale = transform.localScale;
            WorldProperties.currentScale = currentScale;

            actualScale.x = (currentScale.x / originalScale.x);
            actualScale.y = (currentScale.y / originalScale.y);
            actualScale.z = (currentScale.z / originalScale.z);
            WorldProperties.actualScale = actualScale;
        }

        // Makes sure the map sits within the boundaries of the visible table.
        // Checks if two circles (tableCenter w/ radius tableRadius, and mapCenter w/ radius worldMapRadius), intersect
        // If not, then moves the map towards the center until it does.
        private void EnforceMapBoundary()
        {
            Vector3 tableCenter = originalPosition;
            Vector3 mapCenter = transform.position;
            float worldMapRadius = mapRadius * transform.localScale.x;

            // Distance check
            float distSqr = Vector3.SqrMagnitude(tableCenter - mapCenter);
            if (distSqr > Mathf.Pow(tableRadius + worldMapRadius, 2))
            {
                // Peru: 3/7/2020 : Map Integration
                droneOutOfBounds = true;
                // Peru: 3/7/2020 : Map Integration


                // Create vector from mapCenter to edge of table circle (in bounds)
                float distDiff = Vector3.Distance(tableCenter, mapCenter) - tableRadius - worldMapRadius;
                Vector3 movement = Vector3.Normalize(tableCenter - mapCenter) * distDiff;
                movement.y = 0;

                // Move
                transform.Translate(movement, Space.World);
            }
            else
            {
                droneOutOfBounds = false;
            }
        }


    }

}
