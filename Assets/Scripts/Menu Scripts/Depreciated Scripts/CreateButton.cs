namespace ISAACS
{
    using UnityEngine;
    using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities

    public class CreateButton : MonoBehaviour
    {
        Button myButton;

        void Awake()
        {
            myButton = GetComponent<Button>(); // <-- you get access to the button component here

            myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
        }

        void OnClickEvent()
        {
            WorldProperties.NewDrone();
        }
    }
}