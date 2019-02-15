namespace ISAACS
{
    using UnityEngine;
    using UnityEngine.UI; // <-- you need this to access UI (button in this case) functionalities
    using ROSBridgeLib.interface_msgs;
    using VRTK;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    public class LaunchButton : MonoBehaviour
    {
        Button myButton;
        bool flying;
        private GameObject controller; //needed to access pointer

        void Awake()
        {

            controller = GameObject.FindGameObjectWithTag("GameController");

            myButton = GetComponent<Button>(); // <-- you get access to the button component here

            myButton.onClick.AddListener(() => { OnClickEvent(); });  // <-- you assign a method to the button OnClick event here
            
            flying = false;
        }

        private void Update()
        {
            if (flying)
            {
                WorldProperties.runtime += Time.deltaTime;
                
            }
        }

        void OnClickEvent()
        {
            if (controller.GetComponent<VRTK_Pointer>().IsPointerActive())
            {
                if (WorldProperties.selectedDrone != null && !flying)
                {
                    WorldProperties.worldObject.GetComponent<ROSDroneConnection>().SendServiceCall("/takeoff", "");
                    GetComponentInChildren<Text>().text = "Land";
                    flying = true;
                    Debug.Log("Total planning time was: " + WorldProperties.planningTime + "s");

                    string fileName = "Assets/ExperimentData.txt";
                    StreamWriter writer = new StreamWriter(fileName, true);
                    //Debug.Log(runtime);
                    //Debug.Log(planningTime);
                    //Debug.Log("Application end");

                    writer.WriteLine("\r\n");
                    writer.WriteLine("Planning Time: " + WorldProperties.planningTime);
                    //writer.WriteLine("Running Time: " + runtime);
                    //writer.WriteLine(System.DateTime.Now);
                    writer.Close();

                }

                else if (WorldProperties.selectedDrone != null && flying)
                {
                    WorldProperties.worldObject.GetComponent<ROSDroneConnection>().SendServiceCall("/land", "");
                    GetComponentInChildren<Text>().text = "Takeoff";
                    flying = false;
                    Debug.Log("Total flight time was: " + WorldProperties.runtime + "s");

                    string fileName = "Assets/ExperimentData.txt";
                    StreamWriter writer = new StreamWriter(fileName, true);
                    //Debug.Log(runtime);
                    //Debug.Log(planningTime);
                    //Debug.Log("Application end");

                    //writer.WriteLine("\r\n");
                    //writer.WriteLine("Planning Time: " + planningTime);
                    writer.WriteLine("Running Time: " + WorldProperties.runtime);
                    //writer.WriteLine(System.DateTime.Now);
                    writer.Close();
                }
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