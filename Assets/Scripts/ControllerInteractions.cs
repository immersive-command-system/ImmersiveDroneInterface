using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ControllerInteractions : MonoBehaviour {
    //terrain is the textured heightmap
    GameObject terrain;
    //pivot is the center of the table
    GameObject pivot;
    //originalScale is the original localScale of the world
    public Vector3 originalScale;
    //actualScale is the relative localScale of the world in comparison to its original localScale
    public Vector3 actualScale;
    //currentScale is the current localScale
    Vector3 currentScale;
    //This is the original position of the world
    Vector3 originalPosition;
    //This is the 1/10th of the originalScale of the world
    Vector3 minScale;
    //This is the 10 times the originalScale of the world
    Vector3 maxScale;
    //This is the speed at which the drone can be moved at
    float speed;
    //This is the rotating flag
    bool isRotating;
    //This tells us where the controller was when the rotation started
    Vector3 controllerOriginalPosition;



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

        //This adjusts how fast the map can be moved on the x-z plane
        speed = 3;

        //the rotation flag will only be activated when squeezing one of the grip controllers
        isRotating = false;
    }

    void Update() {
        //SCALING WORLD
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            ScaleWorld();
            UpdateScale();
        } else if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            //ROTATING WORLD LEFT
            rotateWorld();
        } else if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            //ROTATING WORLD RIGHT
            rotateWorld();
        }

        //MOVING WORLD
        moveWorld(); 
    }

    private void moveWorld()
    {
        float moveX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        float moveZ = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
        float moveY = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;

        // update map position based on input
        Vector3 position = transform.position;
        position.x += moveX * speed * Time.deltaTime;
        //position.y += moveY * speed * Time.deltaTime;
        position.z += moveZ * speed * Time.deltaTime;
        transform.position = position;
    }

    private void rotateWorld()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            transform.RotateAround(pivot.transform.position, Vector3.up, 1);
            Debug.Log("Rotating based on right controller!");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            transform.RotateAround(pivot.transform.position, Vector3.up, -1);
            Debug.Log("Rotating based on left controller!");
        }
    }

    //private float getRotationAngle(OVRInput.Controller controller)
    //{
    //    Vector3 originalVector = controllerOriginalPosition - pivot.transform.position;
    //    Debug.Log("Original Vector is: " + originalVector);
    //    Vector3 newVector = OVRInput.GetLocalControllerPosition(controller) - pivot.transform.position;
    //    Debug.Log("New Vector is: " + newVector);
    //    Vector2 originalVector2D = new Vector2(originalVector.x, originalVector.z);
    //    Vector2 newVector2D = new Vector2(newVector.x, newVector.z);
    //    float rotate = Mathf.Acos(Vector3.Dot(originalVector2D, newVector2D));
    //    Debug.Log("Angular Rotation speed is: "+rotate);
    //    return rotate;
    //}

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
}
