using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//implement clicking on joystick to turn on labels afterwards

public class TimedTutorialRight : MonoBehaviour {

    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerEvents events;
    private VRTK_ObjectTooltip[] individualTooltips;
    private VRTK_ObjectTooltip triggerTooltip, gripTooltip, joystickTooltip, ATooltip, BTooltip;
   

    [Header("Tooltip Timing Settings")]

    [Tooltip("The number of seconds of the intro before displaying any tooltips.")]
    public float introTiming = 2.0f; //audioSource.clip.length
    [Tooltip("The number of seconds to display the IndexTrigger tooltip.")]
    public float indexTriggerTiming;
    [Tooltip("The number of seconds to display the GripTrigger tooltip.")]
    public float gripTriggerTiming;
    [Tooltip("The number of seconds to display the Joystick tooltip.")]
    public float joystickTiming;
    [Tooltip("The number of seconds to display the A_Select tooltip.")]
    public float A_SelectTiming; //aka Button One Timing
    [Tooltip("The number of seconds to display the B_Delete tooltip.")]
    public float B_DeleteTiming; //aka Button Two Timing


    [Header("PRESSED Tooltip Colour Settings")]

    [Tooltip("The colour to use for the tooltip background container.")]
    public Color tipBackgroundColor_WhenPressed = Color.red;
    [Tooltip("The colour to use for the text within the tooltip.")]
    public Color tipTextColor_WhenPressed = Color.white;
    [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
    public Color tipLineColor_WhenPressed = Color.red;

    [Header("AFTER_PRESSED Tooltip Colour Settings")]

    [Tooltip("The colour to use for the tooltip background container.")]
    public Color tipBackgroundColor_AfterPressed = Color.gray;
    [Tooltip("The colour to use for the text within the tooltip.")]
    public Color tipTextColor_AfterPressed = Color.white;
    [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
    public Color tipLineColor_AfterPressed = Color.gray;


    private bool pressedIndexTrigger = false, pressedGripTrigger = false, pressedJoystick = false, pressedA_Select = false, pressedB_Select = false;


    void Start()
    {
        events = GetComponent<VRTK_ControllerEvents>();
        tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
        individualTooltips = GetComponentsInChildren<VRTK_ObjectTooltip>(true);

        InitializeIndividualTooltips();
        tooltips.ToggleTips(false);
        SetupControllerEventListeners();

        StartCoroutine(TutorialCoroutine());
    }

    private void InitializeIndividualTooltips()
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

    private void SetupControllerEventListeners()
    {
        events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
        /*
        events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
        events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

        events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
        events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

        events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
        */
    }

    IEnumerator TutorialCoroutine()
    {
        yield return new WaitForSecondsRealtime(introTiming);
        tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        yield return new WaitForSecondsRealtime(indexTriggerTiming);

    }

    private void changeTooltipColorWhenPressed(VRTK_ObjectTooltip tooltip, Color tooptipPart)
    {
        tooltip.containerColor = tipBackgroundColor_WhenPressed;
        tooltip.fontColor = tipTextColor_WhenPressed;
        tooltip.lineColor = tipLineColor_WhenPressed;
        tooltip.ResetTooltip();
    }

    private void ChangeContainerColor() { }
    private void ChangeFontColor() { }
    private void ChangeLineColor() { }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    { 
        if (!pressedIndexTrigger)
        {
          //  changeTooltipColor(triggerTooltip);
            pressedIndexTrigger = true;
        }
        else
        {

            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }

        
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
    }

    private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
    {
      //  changeTooltipColor(ATooltip);
    }

    private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
    }

    private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
    {
       // changeTooltipColor(BTooltip);
    }

    private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
    }

    private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
       // changeTooltipColor(gripTooltip);
    }

    private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
     //   changeTooltipColor(joystickTooltip);
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
    }
}
