namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DroneProperties : MonoBehaviour {

        public Drone classPointer;

        // Use this for initialization
        void Start() {
            classPointer = new Drone(gameObject);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
