namespace ISAACS
{
    using UnityEngine;
    using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
    using ROSBridgeLib.interface_msgs;

    public class LaunchButton : MonoBehaviour
    {
        Button myButton;
        bool flying;

        void Awake()
        {
            myButton = GetComponent<Button>(); // <-- you get access to the button component here

            myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here

            flying = false;
        }

        void OnClickEvent()
        {
            if (WorldProperties.selectedDrone != null && !flying)
            {
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().SendServiceCall("/takeoff", "");
                GetComponentInChildren<Text>().text = "Land";
                flying = true;
            }

            else if (WorldProperties.selectedDrone != null && flying)
            {
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().SendServiceCall("/land", "");
                GetComponentInChildren<Text>().text = "Takeoff";
                flying = false;
            }
        }

        void OnApplicationQuit()
        {
            if (WorldProperties.selectedDrone != null && flying)
            {
                WorldProperties.worldObject.GetComponent<ROSDroneConnection>().SendServiceCall("/land", "");
            }
        }
    }
}