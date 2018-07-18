namespace ISAACS
{
    using UnityEngine;
    using VRTK;
    using System.Collections;

    //use this script instead if you only desire the tooltipButton's tooltip to show when corresponding tooltip is pressed, and to disappear when button is released

    public class AutomaticTooltips : MonoBehaviour
    {

        private VRTK_ControllerTooltips tooltips;
        private VRTK_ControllerEvents events;

        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerAppearance_Example", "VRTK_ControllerEvents", "the same"));
                return;
            }

            events = GetComponent<VRTK_ControllerEvents>();
            tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();

            //Setup controller event listeners
            events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
            events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

            events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
            events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

            events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
            events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

            events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
            events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

            events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
            events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

            //  events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
          //  events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

            tooltips.ToggleTips(false);
        }


        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
        }

        private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
        }

       /* private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
       
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
        }
        */


    }
}
