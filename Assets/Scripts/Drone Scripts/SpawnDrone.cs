namespace ISAACS

{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;



    public class SpawnDrone : MonoBehaviour
    {



        public GameObject drone;
        public GameObject mainMenu;


        private GameObject world;
        private GameObject controller;

        //menuState stores if the main menu is currently active

        private bool menuState;

        //toggleMenuStopper

        private bool toggleMenuStopper;
        private bool placingDrone;
        private Vector3 groundPoint;



        // Use this for initialization

        void Start()
        {

            world = GameObject.FindGameObjectWithTag("World");

            controller = GameObject.FindGameObjectWithTag("GameController");

            menuState = false;

            toggleMenuStopper = true;

            mainMenu.SetActive(menuState);

        }



        // Update is called once per frame

        void Update()
        {

            if (OVRInput.Get(OVRInput.Button.One))

            {

                if (toggleMenuStopper)

                {

                    mainMenu.SetActive(!menuState);

                    menuState = !menuState;

                    toggleMenuStopper = false;

                }

            }
            else

            {

                toggleMenuStopper = true;

            }

            if (placingDrone && controller.GetComponent<VRTK_StraightPointerRenderer>().OnGround() && OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) != 0)

            {

                ChooseGroundPoint();

                controller.GetComponent<VRTK_StraightPointerRenderer>().placingDrone();

            }

        }


        // This script is put on a drone placement button
        public void OnClick()

        {

            controller.GetComponent<VRTK_StraightPointerRenderer>().placingDrone();

            placingDrone = true;

            mainMenu.SetActive(false);

            menuState = !menuState;

            GameObject[] drones = GameObject.FindGameObjectsWithTag("Drone");

            foreach (GameObject i in drones)

            {

                i.GetComponent<SetWaypoint>().selected = false;

            }

        }

        // Select the location the drone is placed at
        private void ChooseGroundPoint()

        {

            Vector3 groundPoint = controller.GetComponent<VRTK_StraightPointerRenderer>().GetGroundPoint();

            groundPoint.y = groundPoint.y + 0.5f * world.GetComponent<MapInteractions>().actualScale.y;

            Instantiate(drone, groundPoint, Quaternion.identity, world.transform);

            placingDrone = false;

        }



        // Returns the height taking the scale into account

        private float MaxHeight()

        {

            return 1 + (float)((1.0008874438 * 1.5 - 0.7616114718) * world.GetComponent<MapInteractions>().actualScale.y + .703);

        }

    }

}