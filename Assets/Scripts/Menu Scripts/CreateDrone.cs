namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CreateDrone : MonoBehaviour
    {
        public GameObject droneObject;

        public void newDrone()
        {
            Drone newDrone = new Drone(WorldProperties.worldObject.transform.position + new Vector3 (0,0.5f,0), droneObject);
        } 
    }
}