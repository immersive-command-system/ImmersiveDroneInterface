using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//implement clicking on joystick to turn on labels afterwards

public class TimedTutorialRight : MonoBehaviour {

    private ToggleTooltips individualTooltips;
    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerEvents events;
    private bool pressedTrigger = false, pressedGrip = false, pressedTouchpad = false, pressedButtonOne = false, pressedButtonTwo = false;
    private bool triggerAudioDone = false, gripAudioDone = false, touchpadAudioDone = false, buttonOneAudioDone = false, buttonTwoAudioDone = false;
    public AudioSource introAudio, triggerAudio, gripAudio, touchpadAudio, buttonOneAudio, buttonTwoAudio;
    
    
    [Header("Tooltip Timing Settings")]

    [Tooltip("The number of seconds of the intro before displaying any tooltips.")]
    public float introTiming; //audioSource.clip.length
    [Tooltip("The number of seconds to display the IndexTrigger tooltip.")]
    public float triggerTiming;
    [Tooltip("The number of seconds to display the GripTrigger tooltip.")]
    public float gripTiming;
    [Tooltip("The number of seconds to display the Joystick tooltip.")]
    public float touchpadTiming;
    [Tooltip("The number of seconds to display the A_Select tooltip.")]
    public float buttonOneTiming; //aka Button One Timing
    [Tooltip("The number of seconds to display the B_Delete tooltip.")]
    public float buttonTwoTiming; //aka Button Two Timing


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

        tooltips.ToggleTips(false);
        SetupControllerEventListeners();

        introTiming = introAudio.clip.length;
        triggerTiming = triggerAudio.clip.length;
        touchpadTiming = touchpadAudio.clip.length;

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
        */
        events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
        
    }

    IEnumerator TutorialCoroutine()
    {
        introAudio.Play();
        yield return new WaitForSecondsRealtime(introTiming);

        triggerAudio.Play();
        tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        yield return new WaitForSecondsRealtime(triggerTiming);
        triggerAudioDone = true;

        if (pressedTrigger && triggerAudioDone)
        {
            touchpadAudio.Play();
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        }
        
        //yield return new WaitForSecondsRealtime(indexTriggerTiming);

    }

/*
    IEnumerator TutorialStep(VRTK_ObjectTooltip tooltip, AudioSource tooltipAudio, bool tooltipPressed)
    {
        tooltipAudio.Play();
        switch (tooltip.name.Replace("Tooltip", "").ToLower())
        {
            case "trigger":
                tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
                break;
            case "grip":
                tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
                break;
            case "touchpad":
                tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
                break;
            case "buttonone":
                tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
                break;
            case "buttontwo":
                tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
                break;
        }
        yield return new WaitForSecondsRealtime(tooltipAudio.clip.length);
    }*/

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
    
    private void DoTooltipPressed(bool audioDone, ref bool pressedTooltip, VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
    {
        if (audioDone)
        {
            if (!pressedTooltip)
            {
                changeTooltipColorWhenPressed(tooltip);
                pressedTooltip = true;
                changeTooltipColorAfterPressed(tooltip);
            }
            else
            {
                tooltips.ToggleTips(true, tooltipButton);
            }
        }
    }

    private void DoTooltipReleased(bool audioDone, VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
    {
        if (audioDone)
        {
            tooltips.ToggleTips(false, tooltipButton);
            tooltip.ResetTooltip();
        }
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(triggerAudioDone, ref pressedTrigger, individualTooltips.triggerTooltip, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(triggerAudioDone, individualTooltips.triggerTooltip, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
    }

    private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(buttonOneAudioDone, ref pressedButtonOne, individualTooltips.buttonOne, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
    }

    private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(buttonOneAudioDone, individualTooltips.buttonOne, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
    }

    private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(buttonTwoAudioDone, ref pressedButtonTwo, individualTooltips.buttonTwo, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
    }

    private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(buttonTwoAudioDone, individualTooltips.buttonTwo, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
    }

    private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(gripAudioDone, ref pressedGrip, individualTooltips.gripTooltip, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
    }

    private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(gripAudioDone, individualTooltips.gripTooltip, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(touchpadAudioDone, ref pressedTouchpad, individualTooltips.touchpadTooltip, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(touchpadAudioDone, individualTooltips.touchpadTooltip, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
    }
}
