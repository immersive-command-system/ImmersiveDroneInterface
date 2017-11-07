using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ControllerInteractions : MonoBehaviour {
    //terrain is the textured heightmap
    public GameObject terrain;
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
    //This tells us where the controller was when the rotation started
    private LinkedList<Quaternion> rots;
    private LinkedList<float> times;
    //This tells us if the map is still moving or being dragged
    public enum MapState
    {
        IDLE, MOVING, DRAGGING, STOPPED
    }
    // The radius of the table (assuming the table is circular)
    public float tableRadius;
    // The radius of the map (assuming the map is circular)
    public float mapRadius;

    public MapState mapState;
    public OVRInput.Controller currentController;
    private Vector3 startVec;
    private Quaternion originalRotation;
    private Quaternion movementRotation;
    public float velocityDeltaTime = .1f;
    private float movementDeltaTime;
    private float angleScale = 1;
    private float storedTime;
    private float currSpeed;
    private float previousGet = 0f;

    public Vector3 objectScale;

    // Use this for initialization
    void Start () {
        //Terrain assignment
        terrain = GameObject.Find("/World/manila_mesh");

        //Pivot assignment
        pivot = GameObject.Find("/coffee_table");

        //This provides us with basis to create bounds on scaling and something to return to
        originalScale = transform.localScale;
        originalPosition = transform.position;
        actualScale = new Vector3(1, 1, 1);

        //These are the bounds on scaling
        minScale = Vector3.Scale(originalScale, new Vector3(0.1F, 0.1F, 0.1F));
        maxScale = Vector3.Scale(originalScale, new Vector3(10F, 10F, 10F));

        //list of rotation kept to average as decay reference
        rots = new LinkedList<Quaternion>();
        times = new LinkedList<float>();

        mapState = MapState.IDLE;

        currentController = OVRInput.Controller.LTouch;

    }

    void FixedUpdate() {

        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            //SCALING WORLD
            ScaleWorld();
            UpdateScale();
        }
        else
        {
            SimpleRotateWorld();
            //If only one controller is gripped, we will rotate the world.
            //if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
            //{
            //    //STARTING ROTATION
            //    //Set map to rotation state
            //    mapState = MapState.DRAGGING;
            //    //Mark which controller is causing the rotation
            //    if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            //    {
            //        currentController = OVRInput.Controller.LTouch;
            //    }
            //    else
            //    {
            //        currentController = OVRInput.Controller.RTouch;
            //    }
            //    //Grab controller position in world space
            //    Vector3 currPos = OVRInput.GetLocalControllerPosition(currentController);

            //    //Reinitialize values
            //    originalRotation = terrain.transform.rotation;
            //    startVec = (currPos - terrain.transform.position);
            //    rots.Clear();
            //    times.Clear();
            //    currSpeed = 0;
            //}
            ////ROTATING WORLD
            //if (mapState == MapState.DRAGGING)
            //{
            //    Vector3 currPos = OVRInput.GetLocalControllerPosition(currentController);
            //    //IF THE CONTROLLER IS PRESSED DOWN
            //    if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, currentController) != 0f)
            //    {
            //        Vector3 endVec = (currPos - terrain.transform.position);
            //        Quaternion rot = Quaternion.FromToRotation(startVec, endVec);
            //        terrain.transform.rotation = rot * originalRotation;

            //        // Update lists
            //        rots.AddLast(terrain.transform.rotation);
            //        times.AddLast(Time.time);
            //        while (times.First.Value < Time.time - velocityDeltaTime)
            //        {
            //            rots.RemoveFirst();
            //            times.RemoveFirst();
            //        }
            //    }
            //    //IF WE RELEASE THE CONTROLLER
            //    else if (previousGet != 0f)
            //    {
            //        // Set / Reinitialize values
            //        movementRotation = Quaternion.Inverse(rots.First.Value) * terrain.transform.rotation;
            //        movementDeltaTime = Time.time - times.First.Value;
            //        angleScale = 1;
            //        mapState = MapState.MOVING;
            //    }
            //}
        }


        //MOVING WORLD
        MoveWorld();
        EnforceMapBoundary();
        previousGet = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, currentController);
    }

    // Rotate the world based off of the right thumbstick
    private void SimpleRotateWorld()
    {
        float deltaX = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        transform.RotateAround(pivot.transform.position, Vector3.up, deltaX * rotSpeed * 360 * Time.fixedDeltaTime);
    }

    private void MoveWorld()
    {
        float moveX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        float moveZ = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;

        // update map position based on input
        Vector3 position = transform.position;
        position.x += moveX * speed * Time.deltaTime;
        position.z += moveZ * speed * Time.deltaTime;
        transform.position = position;
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
        objectScale = scalingFactor;

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
        }
    }

    void OnApplicationQuit()
    {
        terrain.transform.localScale = originalScale;
    }

    private void UpdateScale()
    {
        currentScale = transform.localScale;
        //originalScale = world.GetComponent<ControllerInteractions>().originalScale;
        actualScale.x = (currentScale.x / originalScale.x);
        actualScale.y = (currentScale.y / originalScale.y);
        actualScale.z = (currentScale.z / originalScale.z);
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
            // Create vector from mapCenter to edge of table circle (in bounds)
            float distDiff = Vector3.Distance(tableCenter, mapCenter) - tableRadius - worldMapRadius;
            Vector3 movement = Vector3.Normalize(tableCenter - mapCenter) * distDiff;
            movement.y = 0;

            // Move
            transform.Translate(movement, Space.World);
        }
    }
}
