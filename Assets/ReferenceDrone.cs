namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ReferenceDrone : MonoBehaviour
    {
        public GameObject referenceDrone;
        public GameObject movementButton;
        public GameObject destroyDroneButton;
        
        void Start()
        {
            movementButton.GetComponent<ToggleDroneMovement>().referenceDrone = referenceDrone;
            destroyDroneButton.GetComponent<DestroyDrone>().referenceDrone = referenceDrone;
        }
    }
}