namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK;

    public class ToggleDroneMovement : MonoBehaviour
    {

        public Drone thisDrone; // instance of Drone classObject
        public GameObject thisDroneGameObject; // associated Drone gameObject
        private Vector3 size;

        void Start()
        {
            thisDrone = WorldProperties.selectedDrone;
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
            return (thisDrone.waypoints.Count > 1);
        }

        private void ActivateButton()
        {
            this.GetComponentInParent<Transform>().localScale = size;
        }

        private void ChangeText()
        {
            if (thisDrone.gameObjectPointer.GetComponent<MoveDrone>().move)
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
            thisDrone.gameObjectPointer.GetComponent<MoveDrone>().move = !thisDrone.gameObjectPointer.GetComponent<MoveDrone>().move;
        }
    }
}