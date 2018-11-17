namespace ISAACS
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;


    public class Up : MonoBehaviour
    {
        public MenuController menuController;

        Button myButton;
        Drone drone;
        private GameObject controller; //needed to access pointer

        void Awake()
        {
            controller = GameObject.FindGameObjectWithTag("GameController");

            myButton = GetComponent<Button>(); // <-- you get access to the button component here
            myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
        }

        void OnClickEvent()
        {
            Debug.Log("Scroll Up");
            menuController.scrollUp();
        }
    }
}
