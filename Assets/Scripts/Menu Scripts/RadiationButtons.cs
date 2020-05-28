using UnityEngine;
using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
using UnityEngine.SceneManagement;
using System.Collections;
using VRTK;
using ISAACS;

public class RadiationButtons : MonoBehaviour {

    Button myButton;
    Drone drone;
    private GameObject controller; //needed to access pointer

    public bool surface_pointcloud = false;
    public bool Level_0 = false;
    public bool Level_1 = false;
    public bool Level_2 = false;
    public bool Level_3 = false;
    public bool Level_4 = false;
    public bool Level_5 = false;

    void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        myButton = GetComponent<Button>(); // <-- you get access to the button component here
        myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
    }

    void OnClickEvent()
    {
        if (surface_pointcloud)
        {
            Debug.Log("Switching to surface point cloud");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_SurfacePointcloud();
        }

        if (Level_0)
        {
            Debug.Log("Switching to Level 0");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_Colorized_0();
        }

        if (Level_1)
        {
            Debug.Log("Switching to Level 1");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_Colorized_1();
        }

        if (Level_2)
        {
            Debug.Log("Switching to Level 2");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_Colorized_2();
        }


        if (Level_3)
        {
            Debug.Log("Switching to Level 3");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_Colorized_3();
        }

        if (Level_4)
        {
            Debug.Log("Switching to Level 4");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_Colorized_4();
        }

        if (Level_5)
        {
            Debug.Log("Switching to Level 5");
            WorldProperties.worldObject.GetComponent<ROSDroneConnection>().LampSubscribe_Colorized_5();
        }

    }
}
