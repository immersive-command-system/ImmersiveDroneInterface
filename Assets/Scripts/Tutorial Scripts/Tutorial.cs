namespace ISAACS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    public class Tutorial : MonoBehaviour {

        [Tooltip("The amount of time to wait before tutorial starts")]
        public float seconds;

        public static GameObject rightController, leftController;
        public static AudioSource introAudio, envAudio, mapLocationAudio, mapRotationAudio, MapScaleAudio, primaryPlacementAudio,
            grabZoneAudio, intermediatePlacementAudio, selectionPointerAudio, secondaryPlacementAudio, undoAndDeleteAudio;

        private static VRTK_ControllerTooltips leftTooltips, rightTooltips;
        private bool done;

        // Use this for initialization
        void Start() {
            seconds = 4;
            done = false;

            leftTooltips = rightController.GetComponentInChildren<VRTK_ControllerTooltips>();
            rightTooltips = leftController.GetComponentInChildren<VRTK_ControllerTooltips>();

            leftTooltips.ToggleTips(false);
            rightTooltips.ToggleTips(false);

            StartCoroutine(TutorialCoroutine());
        }

        IEnumerator TutorialCoroutine()
        {
            yield return new WaitForSecondsRealtime(seconds);
            introAudio.Play();
            yield return new WaitForSecondsRealtime(introAudio.clip.length);
            envAudio.Play();
            yield return new WaitForSecondsRealtime(envAudio.clip.length);

            yield return TutorialStep(mapLocationAudio, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, MapInteractions.MapState.MOVING, -1, MapInteractions.mapState);
            yield return TutorialStep(mapRotationAudio, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, MapInteractions.MapState.ROTATING, 1, MapInteractions.mapState);
            yield return TutorialStep(MapScaleAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, ControllerInteractions.ControllerState.SCALING, 0, ControllerInteractions.currentControllerState);
            yield return TutorialStep(primaryPlacementAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, ControllerInteractions.ControllerState.PLACING_WAYPOINT, 1, ControllerInteractions.currentControllerState);
            yield return TutorialStep(grabZoneAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, ControllerInteractions.ControllerState.GRABBING, 1, ControllerInteractions.currentControllerState);
            yield return TutorialStep(intermediatePlacementAudio, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip, ControllerInteractions.ControllerState.PLACING_WAYPOINT, 1, ControllerInteractions.currentControllerState);
            yield return TutorialStep(selectionPointerAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, ControllerInteractions.ControllerState.POINTING, 1, ControllerInteractions.currentControllerState);
            yield return TutorialStep(secondaryPlacementAudio, VRTK_ControllerTooltips.TooltipButtons.GripTooltip, ControllerInteractions.ControllerState.POINTING, 1, ControllerInteractions.currentControllerState);
            yield return TutorialStep(undoAndDeleteAudio, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip, ControllerInteractions.ControllerState.DELETING, 1, ControllerInteractions.currentControllerState);

        }

        void CheckAction<T>(AudioSource audio, T var, T state)
        {
            while (audio.isPlaying)
            {
                if (var.Equals(state))
                {
                    done = true;
                    return;
                }
            }
        }

        IEnumerator CheckAction<T> (T var, T state)
        {
            if (!done)
            {
                yield return new WaitUntil(() => var.Equals(state));
            }
            done = false;
        }

        IEnumerator TutorialStep<T> (AudioSource audio, VRTK_ControllerTooltips.TooltipButtons button, T state, int controller, T var)
        { //if controller is -1 then left only, 0 then both, 1 then right controller only
            audio.Play();
            CheckAction(audio, var, state);

            if (controller < 0)
            {
                leftTooltips.ToggleTips(true, button);
            } else if (controller > 0)
            {
                rightTooltips.ToggleTips(true, button);
            } else if (controller == 0)
            {
                leftTooltips.ToggleTips(true, button);
                rightTooltips.ToggleTips(true, button);
            }

            yield return new WaitForSecondsRealtime(audio.clip.length);
            CheckAction(var, state);
        }

    }
}
