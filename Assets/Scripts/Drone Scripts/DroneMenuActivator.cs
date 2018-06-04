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
        public bool menuOn; // indicates whether menu is activated currently

        void Start()
        {
            Debug.Log("DroneMenuActivator.Start()");
            selected = true;
            menuOn = true;
            controller = GameObject.FindGameObjectWithTag("GameController");
            droneMenu = Instantiate(droneMenu);
            droneMenu.SetActive(true);
            droneMenu.GetComponent<ReferenceDrone>().referenceDrone = this.gameObject;

            PositionDroneMenu();
        }

        void Update ()
        {
            // updates disabled
            /*
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
            } */
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
            droneMenu.transform.position = new Vector3(0.75f, 2f, 2.07f);
            droneMenu.transform.eulerAngles = new Vector3(
                droneMenu.transform.eulerAngles.x + 45,
                droneMenu.transform.eulerAngles.y,
                droneMenu.transform.eulerAngles.z
            );
            frameCounter++;
            droneMenu.SetActive(true);
        }

        // Turns the drone menu off, currently disabled
        private void MenuTimer()
        {
            /*
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
            */
        }
    }
}
