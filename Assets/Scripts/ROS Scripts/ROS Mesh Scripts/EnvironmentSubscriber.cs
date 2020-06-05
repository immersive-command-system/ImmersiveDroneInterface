using ISAACS;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;

public class EnvironmentSubscriber : ROSBridgeSubscriber
{



    public static int numberThing = 0;

    public static string GetMessageTopic()
    {
        return "/tf";
    }

    public static string GetMessageType()
    {
        return "tf2_msgs/TFMessage";
    }

    public static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new EnvironmentMsg(msg);
    }

    public static void CallBack(ROSBridgeMsg msg)
    {
        EnvironmentMsg poseMsg = (EnvironmentMsg)msg;
        
        if (poseMsg.id_.Contains("hoop"))
        {
            char numID = poseMsg.id_[poseMsg.id_.Length - 1];
            //Debug.Log("Got a tf message: " + poseMsg.id_[poseMsg.id_.Length - 1]);
            
            GameObject currentHoop = null;

            if (!WorldProperties.hoopsDict.ContainsKey(numID))
            {
                GameObject world = GameObject.FindWithTag("World");
                currentHoop = Object.Instantiate(world.GetComponent<WorldProperties>().torus);
                currentHoop.transform.parent = world.transform;
                WorldProperties.hoopsDict[numID] = currentHoop;


               


                Debug.Log("Made hoop with id: " + numID);
            }
            else
            {
                Debug.Log("im in the else");
                Debug.Log(numID);
                /*
                GameObject newCanvas = (GameObject)Object.Instantiate(Resources.Load("TextCanv"));
                Debug.Log("line1");
                //Getting TextMesh component
                TextMesh tempText = newCanvas.GetComponent<TextMesh>();
                Debug.Log("line2");

                Debug.Log("ADD TEXT PLS");
                //Setting the Name
                if (numID == 1)
                {
                    tempText.text = "Hoop A";
                    numberThing += 1;
                }
                else if (numID == 2)
                {
                    tempText.text = "Hoop B";
                    numberThing += 1;
                }
                else
                {
                    tempText.text = "Hoop C";
                }
                */


                currentHoop = WorldProperties.hoopsDict[numID];
               // newCanvas.transform.parent = currentHoop.transform;
            }

            currentHoop.transform.localPosition = WorldProperties.RosSpaceToWorldSpace(poseMsg.x_, poseMsg.y_, poseMsg.z_) +
                                                    WorldProperties.torusModelOffset;
            currentHoop.transform.localScale = new Vector3(2.7f, 2.7f, 2.7f);
            currentHoop.transform.localRotation = new Quaternion(poseMsg.x_rot_+1, poseMsg.y_rot_, poseMsg.z_rot_, poseMsg.w_rot_);
        }
    }
}
