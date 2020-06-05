﻿using UnityEngine;
using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
using UnityEngine.SceneManagement;
using System.Collections;
using VRTK;
using ISAACS;

public class HomeButton : MonoBehaviour
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
        Debug.Log("Go home, you drunk fool!");
        WorldProperties.worldObject.GetComponent<ROSDroneConnection>().GoHome();
    }
}
