namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DroneProperties : MonoBehaviour
    {
        // DroneProperties acts as a component connection between the Drone classObject and the Drone gameObject.
        public Drone classPointer;

        void Start()
        {
            // We create a new instance of the Drone classObject
            classPointer = new Drone(gameObject);
        }
    }
}
