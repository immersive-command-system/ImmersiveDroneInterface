namespace ISAACS
{
    using UnityEngine;
    using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
    using UnityEngine.SceneManagement;
    using System.Collections;

    public class ClearButton : MonoBehaviour
    {
        Button myButton;
        Drone drone;

        void Awake()
        {
            myButton = GetComponent<Button>(); // <-- you get access to the button component here
            myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
        }

        void OnClickEvent()
        {
            drone = WorldProperties.selectedDrone;
            while (drone.waypoints.Count > 1)
            {
                if (((Waypoint)drone.waypoints[drone.waypoints.Count - 1]).prevPathPoint != null)
                {
                    drone.DeleteWaypoint((Waypoint)drone.waypoints[drone.waypoints.Count - 1]);
                }
            }

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }
}