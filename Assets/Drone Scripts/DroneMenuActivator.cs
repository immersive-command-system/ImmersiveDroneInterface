namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DroneMenuActivator : MonoBehaviour {

        public GameObject drone; // refers to drone object
        public GameObject droneMenu; // refers to Drone Menu UI
        public bool selected;

        private GameObject controller; // refers to the right controller
        private int frameCounter; // used to deactivate menu
        private int menuTimer; // used to deactivate menu
        private bool menuOn; // indicates whether menu is activated currently

        void Start()
        {
            selected = true;
            menuOn = false;
            controller = GameObject.FindGameObjectWithTag("GameController");
            droneMenu = Instantiate(droneMenu);
            droneMenu.SetActive(false);
            droneMenu.GetComponent<ReferenceDrone>().referenceDrone = this.gameObject;
        }

        void Update ()
        {
            if (!drone.activeSelf)
            {
                droneMenu.SetActive(false);
            }

            if (selected)
            {
                MenuTimer();
                if (menuOn)
                {
                    PositionDroneMenu();
                }
            } else
            {
                droneMenu.SetActive(false);
            }
	    }

        // Activates the drone menu
        public void ActivateDroneMenu()
        {
            menuOn = true;
            menuTimer = frameCounter;
        }

        // Determines where the drone menu will appear
        private void PositionDroneMenu()
        {
            Vector3 activatePosition = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z + 0.5f);
            droneMenu.transform.position = activatePosition;
            frameCounter++;
            droneMenu.SetActive(true);
        }

        // Turns the drone menu off
        private void MenuTimer()
        {
            if (menuOn)
            {
                if (menuTimer + 300 < frameCounter)
                {
                    menuOn = false;
                    droneMenu.SetActive(false);
                } else if (controller.GetComponentInParent<VRTK_StraightPointerRenderer>().IsSettingWaypoint()) {
                    menuOn = false;
                    droneMenu.SetActive(false);
                }
            }
        }
    }
}
