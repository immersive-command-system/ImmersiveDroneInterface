using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Show off all the Debug UI components.
public class DebugUISample : MonoBehaviour
{
    bool inMenu;
    private Text sliderText;

    public ISAACS_Master ISAACS_master_controller;

	void Start ()
    {
        //TODO

        DebugUIBuilder.instance.AddButton("Get Auth", GetAuthPressed);
        DebugUIBuilder.instance.AddButton("TakeOff", TakeoffPressed);
        DebugUIBuilder.instance.AddButton("Publish", PublishPressed);
        DebugUIBuilder.instance.AddButton("Execute", ExecutePressed);
        DebugUIBuilder.instance.AddButton("Land", LandPressed);
        DebugUIBuilder.instance.AddButton("Home", HomePressed);

        DebugUIBuilder.instance.Show();
        inMenu = true;
	}

    public void TogglePressed(Toggle t)
    {
        Debug.Log("Toggle pressed. Is on? "+t.isOn);
    }
    public void RadioPressed(string radioLabel, string group, Toggle t)
    {
        Debug.Log("Radio value changed: "+radioLabel+", from group "+group+". New value: "+t.isOn);
    }

    public void SliderPressed(float f)
    {
        Debug.Log("Slider: " + f);
        sliderText.text = f.ToString();
    }

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
    }

    private void GetAuthPressed()
    {
        //Requests ISAACS_master_controller to GetAuthority when GetAuth button is pressed in UI
        Debug.Log("UI Get Auth button was pressed.");
        ISAACS_master_controller.Get_Authority_Request();
    }

    private void TakeoffPressed()
    {
        //Requests ISAACS_master_controller to Takeoff when Takeoff button is pressed in UI
        Debug.Log("UI Takeoff button was pressed.");
        ISAACS_master_controller.Takeoff_Request();
    }

    private void PublishPressed()
    {
        //Requests ISAACS_master_controller to Fly when Fly button is pressed in UI
        Debug.Log("UI Publish button was pressed.");
        ISAACS_master_controller.Publish_Request();
    }

    private void LandPressed()
    {
        //Requests ISAACS_master_controller to Land when Land button is pressed in UI
        Debug.Log("UI Land button was pressed.");
        ISAACS_master_controller.Land_Request();
    }

    private void HomePressed()
    {
        //Requests ISAACS_master_controller to GoHome when Home button is pressed in UI
        Debug.Log("UI Home button was pressed.");
        ISAACS_master_controller.Go_Home_Request();
    }
    private void ExecutePressed()
    {
        //Requests ISAACS_master_controller to GetAuthority when GetAuth button is pressed in UI
        Debug.Log("Execute was pressed.");
        ISAACS_master_controller.Execute_Request();
    }
}
