namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DestroyDrone : MonoBehaviour {

        public GameObject referenceDrone;

        public void OnClick()
        {
            Destroy(referenceDrone);
        }
    }
}
