namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ToggleDroneMovement : MonoBehaviour
    {
        // THIS HANDLED THE OLD MOVEMENT BUTTON
        public GameObject referenceDrone;
        private Vector3 size;

        void Start()
        {
            size = new Vector3(0, 0, 0);
            ActivateButton();
        }

        void Update()
        {            
            if (!HasWaypoints())
            {
                size.x = 0;
                size.y = 0;
                size.z = 0;
            } else
            {
                size.x = 1;
                size.y = 1;
                size.z = 1;
            }
            ChangeText();
            ActivateButton();
        }

        private bool HasWaypoints()
        {
            return (SetWaypoint.waypoints.Count > 1);
        }

        private void ActivateButton()
        {
            this.GetComponentInParent<Transform>().localScale = size;
        }

        private void ChangeText()
        {
            if (referenceDrone.GetComponent<MoveDrone>().move)
            {
                this.GetComponentInChildren<Text>().text = "Stop";
            }
            else
            {
                this.GetComponentInChildren<Text>().text = "Start";
            }
        }

        public void OnClick()
        {
            referenceDrone.GetComponent<MoveDrone>().move = !referenceDrone.GetComponent<MoveDrone>().move;

        }
    }
}