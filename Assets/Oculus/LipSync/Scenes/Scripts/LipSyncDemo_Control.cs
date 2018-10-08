using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncDemo_Control : MonoBehaviour {

    [Tooltip("Key used to rotate the demo object up to 45 degrees to the left.")]
    public KeyCode rotateLeftKey = KeyCode.LeftArrow;
    [Tooltip("Key used to rotate the demo object up to 45 degrees to the right.")]
    public KeyCode rotateRightKey = KeyCode.RightArrow;
    [Tooltip("Key used to reset demo object rotation.")]
    public KeyCode resetRotationKey = KeyCode.DownArrow;

    private float resetRotation = 180.0f;
    private float rotationAmount = 20.0f;
    private float rotationMax = 45.0f;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(rotateLeftKey))
        {
            RotateObject(rotationAmount);
        }
        else if (Input.GetKey(rotateRightKey))
        {
            RotateObject(-rotationAmount);
        }
        else if (Input.GetKey(resetRotationKey))
        {
            RotateObject(resetRotation, true);
        }
    }

    void RotateObject(float amountDegrees, bool absolute = false)
    {
        GameObject target = GameObject.Find("LipSyncMorphTarget_Lips");

        if (target == null)
        {
            // Try for other scene object
            target = GameObject.Find("RobotHead_TextureFlip");
        }

        if (target)
        {
            if (absolute)
            {
                float deltaRotate = amountDegrees - target.transform.eulerAngles.y;
                target.transform.Rotate(Vector3.up * deltaRotate);
            }
            else
            {
                float deltaRotate = Time.deltaTime * amountDegrees;
                if (deltaRotate + target.transform.eulerAngles.y >= resetRotation - rotationMax &&
                    deltaRotate + target.transform.eulerAngles.y <= resetRotation + rotationMax)
                {
                    target.transform.Rotate(Vector3.up * deltaRotate);
                }
            }
        }
    }
}
