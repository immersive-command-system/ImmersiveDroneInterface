using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TimedTutorial : MonoBehaviour {

    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerEvents events;
    private VRTK_ObjectTooltip[] individualTooltips;
    private VRTK_ObjectTooltip triggerTooltip, gripTooltip, joystickTooltip, ATooltip, BTooltip;
   

    [Header("Tooltip Timing Settings")]

    [Tooltip("The number of seconds of the intro before displaying any tooltips.")]
    public float intro = 2.0f;
    [Tooltip("The number of seconds to display the IndexTrigger tooltip.")]
    public float indexTrigger;
    [Tooltip("The number of seconds to display the GripTrigger tooltip.")]
    public float gripTrigger;
    [Tooltip("The number of seconds to display the Joystick tooltip.")]
    public float joystick;
    [Tooltip("The number of seconds to display the A_Select tooltip.")]
    public float A_Select;
    [Tooltip("The number of seconds to display the B_Delete tooltip.")]
    public float B_Delete;

    [Header("PRESSED Tooltip Colour Settings")]

    [Tooltip("The colour to use for the tooltip background container.")]
    public Color tipBackgroundColor_WhenPressed = Color.red;
    [Tooltip("The colour to use for the text within the tooltip.")]
    public Color tipTextColor_WhenPressed = Color.white;
    [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
    public Color tipLineColor_WhenPressed = Color.red;

    void Start()
    {
        events = GetComponent<VRTK_ControllerEvents>();
        tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
        individualTooltips = GetComponentsInChildren<VRTK_ObjectTooltip>(true);

        initializeIndividualTooltips();
        tooltips.ToggleTips(false);
        setupControllerEventListeners();

        StartCoroutine(TutorialCoroutine());
    }

    private void initializeIndividualTooltips()
    {
        for (int i = 0; i < individualTooltips.Length; i++)
        {
            VRTK_ObjectTooltip tooltip = individualTooltips[i];

            switch (tooltip.name.Replace("Tooltip", "").ToLower())
            {
                case "trigger":
                    triggerTooltip = tooltip;
                    break;
                case "grip":
                    gripTooltip = tooltip;
                    break;
                case "touchpad":
                    joystickTooltip = tooltip;
                    break;
                case "buttonone":
                    ATooltip = tooltip;
                    break;
                case "buttontwo":
                    BTooltip = tooltip;
                    break;
            }
        }
    }

    private void setupControllerEventListeners()
    {
        events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

        events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
        events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

        events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
        events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

        events.StartMenuPressed += new ControllerInteractionEventHandler(DoStartMenuPressed);
        events.StartMenuReleased += new ControllerInteractionEventHandler(DoStartMenuReleased);

        events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
    }

    IEnumerator TutorialCoroutine()
    {
        print("start");
        yield return new WaitForSecondsRealtime(intro);
        print("after waiting");
        tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);

    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        triggerTooltip.containerColor = tipBackgroundColor_WhenPressed;
        triggerTooltip.fontColor = tipBackgroundColor_WhenPressed;
        triggerTooltip.lineColor = tipLineColor_WhenPressed;

       // tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
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

    private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
    }

    private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
    }

    private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
    }

    private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
    }
}
