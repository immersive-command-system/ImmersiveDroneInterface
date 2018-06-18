namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    public class HandleInteraction : MonoBehaviour
    {

        public MapInteractions controllerInteractions;

        // Use this for initialization
        void Start()
        {
            GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += OnUsed;
        }

        private void OnUsed(object sender, InteractableObjectEventArgs e)
        {
            controllerInteractions.handleHeldTrigger = true;
            controllerInteractions.currentController = e.interactingObject.name.Equals("RightController") ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;
        }
    }

}
