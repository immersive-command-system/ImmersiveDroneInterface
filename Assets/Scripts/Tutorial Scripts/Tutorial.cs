namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    public class Tutorial : MonoBehaviour {

       /* public static class ControllerStateRef { public static ControllerInteractions.ControllerState getValue() { return ControllerInteractions.currentControllerState; } }
        public static class MapStateRef { public static MapInteractions.MapState getValue() { return MapInteractions.mapState; } }
        public class AudioRef
        {
            private AudioSource backing;
            public AudioSource Value { get { return backing; } set { backing = value; } }
            public AudioRef(AudioSource reference) { backing = reference; }
        } */

        [Tooltip("The amount of time to wait before tutorial starts")]
        public float seconds;

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

        public GameObject rightController, leftController;
        public AudioSource introAudio, envAudio, mapLocationAudio, mapRotationAudio, MapScaleAudio, primaryPlacementAudio,
            grabZoneAudio, intermediatePlacementAudio, selectionPointerAudio, secondaryPlacementAudio, undoAndDeleteAudio;

        private static VRTK_ControllerTooltips leftTooltips, rightTooltips;
        public static bool stepFinished;
        private static Tooltips rightToggling, leftToggling;

        public enum TutorialState  {NONE, INTRO, MOVINGMAP, ROTATINGMAP, SCALINGMAP, PRIMARYPLACEMENT, GRABZONE, INTERMEDIATEPLACEMENT, SELECTIONPOINTER, SECONDARYPLACEMENT, UNDOANDDELETE, DONE};
        public static TutorialState currentTutorialState;

        void Start() {
            seconds = 4;
            stepFinished = false;

            rightTooltips = rightController.GetComponentInChildren<VRTK_ControllerTooltips>();
            leftTooltips = leftController.GetComponentInChildren<VRTK_ControllerTooltips>();
            rightToggling = rightController.GetComponent<Tooltips>();
            leftToggling = leftController.GetComponent<Tooltips>();

            leftTooltips.ToggleTips(false);
            rightTooltips.ToggleTips(false);

            currentTutorialState = TutorialState.NONE;

            StartCoroutine(TutorialCoroutine());
        }

        IEnumerator TutorialCoroutine()
        {
            yield return new WaitForSecondsRealtime(seconds);
            currentTutorialState = TutorialState.INTRO;
            introAudio.Play();
            yield return new WaitForSecondsRealtime(introAudio.clip.length);
            envAudio.Play();
            yield return new WaitForSecondsRealtime(envAudio.clip.length);

        /*    currentTutorialState = TutorialState.MOVINGMAP;
            yield return StartCoroutine(TutorialStep(mapLocationAudio, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, null, -1));

            currentTutorialState = TutorialState.ROTATINGMAP;
            yield return StartCoroutine(TutorialStep(mapRotationAudio, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, null, 1));

            currentTutorialState = TutorialState.SCALINGMAP;
            yield return StartCoroutine(TutorialStep(MapScaleAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, null, 0));
            */
            currentTutorialState = TutorialState.PRIMARYPLACEMENT;
            yield return StartCoroutine(TutorialStep(primaryPlacementAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, null, 1));

            currentTutorialState = TutorialState.GRABZONE;
            yield return StartCoroutine(TutorialStep(grabZoneAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, null, 1));

            currentTutorialState = TutorialState.INTERMEDIATEPLACEMENT;
            yield return StartCoroutine(TutorialStep(intermediatePlacementAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, null, 1));

            currentTutorialState = TutorialState.SELECTIONPOINTER;
            yield return StartCoroutine(TutorialStep(selectionPointerAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, null, 1));

            currentTutorialState = TutorialState.SECONDARYPLACEMENT;
            yield return StartCoroutine(TutorialStep(secondaryPlacementAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, rightToggling.ChangeButtonToTooltip(VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip), 1));

            currentTutorialState = TutorialState.UNDOANDDELETE;
            yield return StartCoroutine(TutorialStep(undoAndDeleteAudio, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip, null, 1));

            currentTutorialState = TutorialState.DONE;
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
        IEnumerator TutorialStep (AudioSource audio, VRTK_ControllerTooltips.TooltipButtons button, VRTK_ObjectTooltip tooltip, int controller)
        { //if controller is -1 then left only, 0 then both, 1 then right controller only
            audio.Play();

            if (controller <= 0) {
                ChangeTooltipColorDuringTutorialStep(leftToggling, leftToggling.ChangeButtonToTooltip(button));
                leftTooltips.ToggleTips(true, button);
            } else if (controller >= 0) {
                if (tooltip != null) {
                    ChangeTooltipColorDuringTutorialStep(rightToggling, tooltip);
                    rightTooltips.ToggleTips(true, rightToggling.ChangeTooltipToButton(tooltip));
                }
                ChangeTooltipColorDuringTutorialStep(rightToggling, rightToggling.ChangeButtonToTooltip(button));
                rightTooltips.ToggleTips(true, button);
            }
            //CheckAction(audio, currentState, state);
            yield return new WaitForSecondsRealtime(audio.clip.length);

            //yield return StartCoroutine(CheckAction(currentState, state));

            yield return new WaitUntil(() => stepFinished);
            if (controller <= 0)
            {
                ChangeTooltipColorAfterTutorialStep(leftToggling, leftToggling.ChangeButtonToTooltip(button));
                leftTooltips.ToggleTips(false, button);
            }
            else if (controller >= 0)
            {
                if (tooltip != null)
                {
                    ChangeTooltipColorAfterTutorialStep(rightToggling, tooltip);
                    rightTooltips.ToggleTips(false, rightToggling.ChangeTooltipToButton(tooltip));
                }
                ChangeTooltipColorAfterTutorialStep(rightToggling, rightToggling.ChangeButtonToTooltip(button));
                rightTooltips.ToggleTips(false, button);
            }
            stepFinished = false;
        }

        public void ChangeTooltipColorDuringTutorialStep(Tooltips toggling, VRTK_ObjectTooltip tooltip)
        {
            toggling.ChangeContainerColor(tooltip, tipBackgroundColor_DuringTutorialStep);
            toggling.ChangeLineColor(tooltip, tipLineColor_DuringTutorialStep);
            toggling.ChangeFontColor(tooltip, tipTextColor_DuringTutorialStep);
            tooltip.ResetTooltip();
        }

        public void ChangeTooltipColorAfterTutorialStep(Tooltips toggling, VRTK_ObjectTooltip tooltip)
        {
            toggling.ChangeContainerColor(tooltip, tipBackgroundColor_AfterTutorialStep);
            toggling.ChangeLineColor(tooltip, tipLineColor_AfterTutorialStep);
            toggling.ChangeFontColor(tooltip, tipTextColor_AfterTutorialStep);
            tooltip.ResetTooltip();
        }

        private void Update()
        {
            switch (currentTutorialState)
            {
                case Tutorial.TutorialState.MOVINGMAP:
                    if (MapInteractions.mapState == MapInteractions.MapState.MOVING) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.ROTATINGMAP:
                    if (MapInteractions.mapState == MapInteractions.MapState.ROTATING) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.SCALINGMAP:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.SCALING) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.PRIMARYPLACEMENT:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.POINTING) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.GRABZONE:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.GRABBING) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.INTERMEDIATEPLACEMENT:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.PLACING_WAYPOINT) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.SELECTIONPOINTER:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.POINTING) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.SECONDARYPLACEMENT:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.SETTING_HEIGHT) {
                        stepFinished = true;
                    }
                    break;
                case Tutorial.TutorialState.UNDOANDDELETE:
                    if (ControllerInteractions.currentControllerState == ControllerInteractions.ControllerState.DELETING) {
                        stepFinished = true;
                    }
                    break;
                    
            }
        }
    }
}
