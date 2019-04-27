namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK;
    //using UnityEngine.SceneManagement;

    /// <summary>
    /// This class manages the tutorial for how to navigate the ISAACS interface.
    /// </summary>
    /// <remarks>
    /// This script needs to be added to the parent (i.e. Controllers) of the relevant alias controller GameObjects (i.e. RightController, LeftController)
    /// </remarks>
    public class Tutorial : MonoBehaviour
    {

        /* public static class ControllerStateRef { public static ControllerInteractions.ControllerState getValue() { return ControllerInteractions.currentControllerState; } }
         public static class MapStateRef { public static MapInteractions.MapState getValue() { return MapInteractions.mapState; } }
         public class AudioRef
         {
             private AudioSource backing;
             public AudioSource Value { get { return backing; } set { backing = value; } }
             public AudioRef(AudioSource reference) { backing = reference; }
         } */

        [Tooltip("The amount of time to wait before tutorial starts")]
        public float seconds = 4;

        [Header("DURING TutorialStep Tooltip Colour Settings")]

        [Tooltip("The colour to use for the tooltip background container.")]
        public Color tipBackgroundColor_DuringTutorialStep = Color.green;
        [Tooltip("The colour to use for the text within the tooltip.")]
        public Color tipTextColor_DuringTutorialStep = Color.white;
        [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
        public Color tipLineColor_DuringTutorialStep = Color.green;

        [Header("AFTER TutorialStep Tooltip Colour Settings")]

        [Tooltip("The colour to use for the tooltip background container.")]
        public Color tipBackgroundColor_AfterTutorialStep = Color.blue;
        [Tooltip("The colour to use for the text within the tooltip.")]
        public Color tipTextColor_AfterTutorialStep = Color.white;
        [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
        public Color tipLineColor_AfterTutorialStep = Color.blue;



        public GameObject rightController, leftController; //the controllers that have the Tooltips script attached to each of them
        public AudioSource introAudio, envAudio, mapLocationAudio, mapRotationAudio, MapScaleAudio, primaryPlacementAudio,
            grabZoneAudio, intermediatePlacementAudio, selectionPointerAudio, secondaryPlacementAudio, undoAndDeleteAudio, tutorialDoneAudio, removeOrExploreAudio;

        private static VRTK_ControllerTooltips leftTooltips, rightTooltips;
        private static bool stepFinished; //lets you know when the tutorial can move on to the next step
        private static Tooltips rightToggling, leftToggling; //used to toggle corresponding tooltips of the controllers

        //possible states of the tutorial AKA the tutorialsteps
        public enum TutorialState { NONE, INTRO, MOVINGMAP, ROTATINGMAP, SCALINGMAP, PRIMARYPLACEMENT, GRABZONE, INTERMEDIATEPLACEMENT, SELECTIONPOINTER, SECONDARYPLACEMENT, UNDOANDDELETE, DONE };
        public static TutorialState currentTutorialState; //keeps track of the current tutorial state

        Text instruction;
        /// <summary>
        /// Initializes all necessary variables and starts the tutorial
        /// </summary>
        void Start()
        {
            stepFinished = false;

            rightTooltips = rightController.GetComponentInChildren<VRTK_ControllerTooltips>();
            leftTooltips = leftController.GetComponentInChildren<VRTK_ControllerTooltips>();
            rightToggling = rightController.GetComponent<Tooltips>();
            leftToggling = leftController.GetComponent<Tooltips>();

            leftTooltips.ToggleTips(false);
            rightTooltips.ToggleTips(false);

            currentTutorialState = TutorialState.NONE;

            instruction = GameObject.Find("InstructionCanvas/InstructionText").GetComponent<Text>();

            StartCoroutine(TutorialCoroutine());
        }

        /// <summary>
        /// Initiates each step of the tutorial sequentially, waiting for the previous step to finish before starting the next
        /// </summary>
        /// <returns></returns>
        IEnumerator TutorialCoroutine()
        {
            //dunno why, but this is needed for the tooltips to be pointed to the right places for the right controller
            rightController.transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(1);
            rightController.transform.GetChild(0).gameObject.SetActive(true); 

            yield return new WaitForSecondsRealtime(seconds);

            currentTutorialState = TutorialState.INTRO;
            introAudio.Play();
            instruction.text = "Welcome to the Immersive Semi-Autonomous Aerial Command System, also known as ISAACS. ISAACS is an open-source drone command platform, designed for immersive interfaces such as the Oculus Rift. This tutorial will show you how to start working with the interface and use it as an intuitive and seamless extension of  your drone supervision capabilities.";
            yield return new WaitForSecondsRealtime(introAudio.clip.length);

            envAudio.Play();
            instruction.text = "On the table in front of you,  you can see a height map and scan of the environment that the drone will be flying in. This environment can be anywhere in the world and enables an operator to have a sense of presence for where the drone is.";
            yield return new WaitForSecondsRealtime(envAudio.clip.length);

            currentTutorialState = TutorialState.MOVINGMAP;
            instruction.text = "Use your left joystick to change the location of the map.";
            yield return StartCoroutine(TutorialStep(mapLocationAudio, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, null, -1));

            currentTutorialState = TutorialState.ROTATINGMAP;
            instruction.text = "Use your right joystick to rotate the map.";
            yield return StartCoroutine(TutorialStep(mapRotationAudio, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, null, 1));

            currentTutorialState = TutorialState.SCALINGMAP;
            instruction.text = "Grab both of your grip triggers at the same time and pull your hands outward or push them together to change scale of entire map.";
            yield return StartCoroutine(TutorialStep(MapScaleAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, null, 0));

            currentTutorialState = TutorialState.PRIMARYPLACEMENT;
            instruction.text = "“In front of your right controller you should be able to see a white sphere. This is called the placepoint. Press down the index trigger to create a waypoint at the placepoint location. You can drag this waypoint around as long as the index trigger is held. Release the index trigger to finalize the waypoint location.";
            yield return StartCoroutine(TutorialStep(primaryPlacementAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, null, 1));

            currentTutorialState = TutorialState.GRABZONE;
            instruction.text = "At the end of your right controller surrounding your placepoint is a blue sphere called the grab zone. When you move the sphere to collide with a waypoint, the waypoint will highlight to show that it is possible to move it. To move a highlighted waypoint, simply hold down your index trigger and move the controller - the waypoint will follow.";
            yield return StartCoroutine(TutorialStep(grabZoneAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, null, 1));

            currentTutorialState = TutorialState.INTERMEDIATEPLACEMENT;
            instruction.text = "The grab zone can also be used to place intermediate waypoints. If you move your grab zone such that it is colliding with a line segment in the waypoint path, it will indicate that you wish to place an intermediate waypoint. If you attempt to place a waypoint using your index trigger while highlighting a path segment, the new waypoint will be placed between the existing waypoints at either end of that path segment.";
            yield return StartCoroutine(TutorialStep(intermediatePlacementAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, null, 1));

            currentTutorialState = TutorialState.SELECTIONPOINTER;
            instruction.text = "“Holding down grip trigger without a waypoint or line highlighted will cause our selection pointer to appear. This pointer will help you select buttons, locations and objects that are far away from you.";
            yield return StartCoroutine(TutorialStep(selectionPointerAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, null, 1));

            currentTutorialState = TutorialState.SECONDARYPLACEMENT;
            instruction.text = "The selection pointer will enable you to use the secondary waypoint placement method. Hold down your grip trigger and point at the terrain, then press the index trigger to create a ground point. Tilt controller to finalize the waypoint height and click the index trigger again to set the waypoint.";
            yield return StartCoroutine(TutorialStep(secondaryPlacementAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, rightToggling.ChangeButtonToTooltip(VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip), 1));

            currentTutorialState = TutorialState.UNDOANDDELETE;
            instruction.text = "Pressing the B button on your right controller will delete the last waypoint that you placed. To delete particular waypoint, highlight it by putting it in your grab zone and then hit the B button.";
            yield return StartCoroutine(TutorialStep(undoAndDeleteAudio, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip, null, 1));

            currentTutorialState = TutorialState.DONE;

            tutorialDoneAudio.Play();
            instruction.text = "Thank you for completing the tutorial.";
            yield return new WaitForSecondsRealtime(tutorialDoneAudio.clip.length);
            removeOrExploreAudio.Play();
            instruction.text = "You may remove your headset or continue to experiment in the interface.";

        }

        /*    void CheckAction<T>(AudioRef audio, Func<T> currentState, T state)
             {
                 bool isPlaying = audio.Value.isPlaying;
                 while (isPlaying)
                 {
                     if (currentState().Equals(state))
                     {
                         Debug.Log(currentState());
                         stepFinished = true;
                         return;
                     }
                     isPlaying = audio.Value.isPlaying;
                 }
             }

             IEnumerator CheckAction<T> (Func<T> currentState, T state)
             {
                 if (!stepFinished)
                 {
                     Debug.Log(currentState());
                     yield return new WaitUntil(() => currentState().Equals(state));
                 }
                 stepFinished = false;
             }
             */

        /// <summary>
        /// Plays the audio of the tutorial step, and shows the tooltip of the corresponding button and additional givin tooltip if there is one.
        /// Waits until does the task mentioned in the tutorial step.
        /// </summary>
        /// <param name="audio">The audio to play for the tutorial step</param>
        /// <param name="button">The button that is required for the tutorial step, of which its tooltip should show</param>
        /// <param name="tooltip">Additional tooltip object that should show if needed for the tutorial step, null if only one tooltip is needed.</param>
        /// <param name="controller">If controller is -1 then left only, 0 then both, 1 then right controller only</param>
        /// <returns></returns>
        IEnumerator TutorialStep(AudioSource audio, VRTK_ControllerTooltips.TooltipButtons button, VRTK_ObjectTooltip tooltip, int controller)
        {
            audio.Play();

            //change tooltip colors to the color during tutorial step 
            if (controller <= 0)
            {
                ChangeTooltipColorDuringTutorialStep(leftToggling, leftToggling.ChangeButtonToTooltip(button));
                leftTooltips.ToggleTips(true, button);
            }
            if (controller >= 0)
            {
                if (tooltip != null)
                {
                    ChangeTooltipColorDuringTutorialStep(rightToggling, tooltip);
                    rightTooltips.ToggleTips(true, rightToggling.ChangeTooltipToButton(tooltip));
                }
                ChangeTooltipColorDuringTutorialStep(rightToggling, rightToggling.ChangeButtonToTooltip(button));
                rightTooltips.ToggleTips(true, button);
            }
            //CheckAction(audio, currentState, state);
            yield return new WaitForSecondsRealtime(audio.clip.length);

            //yield return StartCoroutine(CheckAction(currentState, state));

            yield return new WaitUntil(() => stepFinished); //wait until step is finished before continuing tutorial

            //change tooltip colors once step is finished
            if (controller <= 0)
            {
                ChangeTooltipColorAfterTutorialStep(leftToggling, leftToggling.ChangeButtonToTooltip(button));
                leftTooltips.ToggleTips(false, button);
            }
            if (controller >= 0)
            {
                if (tooltip != null)
                {
                    ChangeTooltipColorAfterTutorialStep(rightToggling, tooltip);
                    rightTooltips.ToggleTips(false, rightToggling.ChangeTooltipToButton(tooltip));
                }
                ChangeTooltipColorAfterTutorialStep(rightToggling, rightToggling.ChangeButtonToTooltip(button));
                rightTooltips.ToggleTips(false, button);
            }

            //set it back to false for the next step
            stepFinished = false;
        }

        /// <summary>
        /// Changes the colors of the tooltip's container, line, and font for during a tutorial step
        /// </summary>
        /// <param name="toggling">The Tooltips script instance to use for toggling the tooltips of the corresponding controller the script is attached to</param>
        /// <param name="tooltip">The tooltip the change the color of</param>
        public void ChangeTooltipColorDuringTutorialStep(Tooltips toggling, VRTK_ObjectTooltip tooltip)
        {
            toggling.ChangeContainerColor(tooltip, tipBackgroundColor_DuringTutorialStep);
            toggling.ChangeLineColor(tooltip, tipLineColor_DuringTutorialStep);
            toggling.ChangeFontColor(tooltip, tipTextColor_DuringTutorialStep);
            tooltip.ResetTooltip();
        }

        /// <summary>
        /// Changes the colors of the tooltip's container, line, and font for after a tutorial step
        /// </summary>
        /// <param name="toggling">The Tooltips script instance to use for toggling the tooltips of the corresponding controller the script is attached to</param>
        /// <param name="tooltip">The tooltip to change the color of</param>
        public void ChangeTooltipColorAfterTutorialStep(Tooltips toggling, VRTK_ObjectTooltip tooltip)
        {
            toggling.ChangeContainerColor(tooltip, tipBackgroundColor_AfterTutorialStep);
            toggling.ChangeLineColor(tooltip, tipLineColor_AfterTutorialStep);
            toggling.ChangeFontColor(tooltip, tipTextColor_AfterTutorialStep);
            tooltip.ResetTooltip();
        }

        /// <summary>
        /// Checks if the user does what the turial step indicates to do during the tutorial step, so that the tutorial can move on to the next step
        /// </summary>
        private void Update()
        {
            switch (currentTutorialState)
            {
                case Tutorial.TutorialState.MOVINGMAP:
                    if (MapInteractions.mapState == MapInteractions.MapState.MOVING)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.ROTATINGMAP:
                    if (MapInteractions.mapState == MapInteractions.MapState.ROTATING)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.SCALINGMAP:
                    if (MapInteractions.mapState == MapInteractions.MapState.SCALING)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.PRIMARYPLACEMENT:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.PLACING_WAYPOINT)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.GRABZONE:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.GRABBING)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.INTERMEDIATEPLACEMENT:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.PLACING_WAYPOINT)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.SELECTIONPOINTER:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.POINTING)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.SECONDARYPLACEMENT:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.SETTING_HEIGHT)
                    {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.UNDOANDDELETE:
                    //having issues with ControllerState.DELETING on when to set it back to IDLE, so for now since this is the last tutorial step, just going to have it check if user hits B-button
                    //if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.DELETING) { 
                    if (OVRInput.GetDown(OVRInput.Button.Two)) {
                            stepFinished = true;
                    }
                    break;
                    /* case Tutorial.TutorialState.DONE:
                         Debug.Log("loading Main scene");
                         VRTK_SDKManager.instance.UnloadSDKSetup();
                         SceneManager.LoadScene("Main");
                         break;
                         */
            }
        }
    }
}