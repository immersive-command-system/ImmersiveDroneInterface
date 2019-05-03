namespace ISAACS
{
    using UnityEngine;
    using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
    using System.Collections;
    using VRTK;

    public class FuncSelectButton : MonoBehaviour
    {
        Button myButton;
        private GameObject controller; //needed to access pointer

        private bool selected = false;

        void Awake()
        {
            controller = GameObject.FindGameObjectWithTag("GameController");

            myButton = GetComponent<Button>(); // <-- you get access to the button component here
            myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
        }

        /// <summary>
        /// On button click, toggle button appearance.
        /// </summary>

        void OnClickEvent()
        {
            if (controller.GetComponent<VRTK_Pointer>().IsPointerActive())
            {
                Debug.Log("clicked!!!!!");
                GameObject selectLabel = transform.GetChild(0).GetChild(0).gameObject;
                if (!selected)
                {
                    selectLabel.GetComponent<Text>().text = "selected";
                    selected = true;
                    
                } else
                {
                    selectLabel.GetComponent<Text>().text = "";
                    selected = false;
                }
                
            }
        }
    }
}
