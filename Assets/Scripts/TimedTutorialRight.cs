using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//implement clicking on joystick to turn on labels afterwards

public class TimedTutorialRight : MonoBehaviour {

    private GameObject audio;
    private ToggleTooltips individualTooltips;
    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerEvents events;
    private bool pressedIndexTrigger = false, pressedGripTrigger = false, pressedJoystick = false, pressedA_Select = false, pressedB_Select = false;

    
    [Header("Tooltip Timing Settings")]

    [Tooltip("The number of seconds of the intro before displaying any tooltips.")]
    public float introTiming; //audioSource.clip.length
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




    void Start()
    {
        events = GetComponent<VRTK_ControllerEvents>();
        individualTooltips = GetComponent<ToggleTooltips>();
        tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
        audio = GetComponentInParent<GameObject>();

        tooltips.ToggleTips(false);
        SetupControllerEventListeners();


        //introTiming = this.transform.parent.Find("TutorialAudio").GetComponent<GameObject>();
        
        StartCoroutine(TutorialCoroutine());
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

    private void changeTooltipColorWhenPressed(VRTK_ObjectTooltip tooltip)
    {
        individualTooltips.ChangeContainerColor(tooltip, tipBackgroundColor_WhenPressed);
        individualTooltips.ChangeLineColor(tooltip, tipLineColor_WhenPressed);
        individualTooltips.ChangeFontColor(tooltip, tipTextColor_WhenPressed);
        tooltip.ResetTooltip();
    }

    private void changeTooltipColorAfterPressed(VRTK_ObjectTooltip tooltip)
    {
        individualTooltips.ChangeContainerColor(tooltip, tipBackgroundColor_AfterPressed);
        individualTooltips.ChangeLineColor(tooltip, tipLineColor_AfterPressed);
        individualTooltips.ChangeFontColor(tooltip, tipTextColor_AfterPressed);
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    { 
        if (!pressedIndexTrigger)
        {
            changeTooltipColorWhenPressed(individualTooltips.triggerTooltip);
            pressedIndexTrigger = true;
            changeTooltipColorAfterPressed(individualTooltips.triggerTooltip);
        }
        else
        {
            individualTooltips.triggerTooltip.ResetTooltip();
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
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
