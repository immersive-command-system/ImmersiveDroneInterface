using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ControllerInteractions : MonoBehaviour {

    GameObject terrain;
    GameObject pivot;
    public Vector3 originalScale;
    public Vector3 actualScale;
    Vector3 currentScale;
    Vector3 originalPosition;
    Vector3 minScale;
    Vector3 maxScale;
    float speed;
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

        //These are the bounds on scaling
        minScale = Vector3.Scale(originalScale, new Vector3(0.1F, 0.1F, 0.1F));
        maxScale = Vector3.Scale(originalScale, new Vector3(10F, 10F, 10F));

        //This adjusts how fast the map can be moved on the x-z plane
        speed = 3;
    }

    // Update is called once per frame
    void Update () {

        //SCALING FUNCTION
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
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
            if (scalingFactor.sqrMagnitude > minScale.sqrMagnitude && scalingFactor.sqrMagnitude < maxScale.sqrMagnitude) {

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

                //This scales World and all its child objects
                //transform.localScale = Vector3.Scale(transform.localScale, new Vector3(final_result, final_result, final_result));
            }
        }

        //MOVING SCENE

        float moveX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        float moveZ = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
        float moveY = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        float rotate = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        

        // update map position based on input
        Vector3 position = transform.position;
        position.x += moveX * speed * Time.deltaTime;
        //position.y += moveY * speed * Time.deltaTime;
        position.z += moveZ * speed * Time.deltaTime;
        transform.position = position;

        transform.RotateAround(pivot.transform.position, Vector3.up, rotate * Time.deltaTime * speed * 30);
        //transform.Rotate(0, rotate*Time.deltaTime*speed*30, 0, Space.Self);

        UpdateScale();
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
