using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//implement clicking on joystick to turn on labels afterwards

public class Ref
{
    private bool backing;
    public bool Value { get { return backing; } set { backing = value; } }
    public Ref()
    {
        backing = false;
    }
}


public class TimedTutorialRight : MonoBehaviour {

    private ToggleTooltips individualTooltips;
    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerEvents events;

    private VRTK_ControllerTooltips.TooltipButtons triggerButton, gripButton, touchpadButton, buttonOneButton, buttonTwoButton;

    private Ref pressedTrigger = new Ref(), pressedGrip = new Ref(), pressedTouchpad = new Ref(), pressedButtonOne = new Ref(), pressedButtonTwo = new Ref();
    //private bool pressedTrigger = false, pressedGrip = false, pressedTouchpad = false, pressedButtonOne = false, pressedButtonTwo = false;
    private Ref triggerAudioDone = new Ref(), gripAudioDone = new Ref(), touchpadAudioDone = new Ref(), buttonOneAudioDone = new Ref(), buttonTwoAudioDone = new Ref();
    //private bool triggerAudioDone = false, gripAudioDone = false, touchpadAudioDone = false, buttonOneAudioDone = false, buttonTwoAudioDone = false;
    public AudioSource introAudio, triggerAudio, gripAudio, touchpadAudio, buttonOneAudio, buttonTwoAudio;

    public float introTiming;
    /* 
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
      */

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

        SetupControllerEventListeners();

        introTiming = introAudio.clip.length;

        triggerButton = VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip;
        gripButton = VRTK_ControllerTooltips.TooltipButtons.GripTooltip;
        touchpadButton = VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip;
        buttonOneButton = VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip;
        buttonTwoButton = VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip;

        StartCoroutine(TutorialCoroutine());
    }

    IEnumerator TutorialCoroutine()
    {

        yield return new WaitForSecondsRealtime(2);

        introAudio.Play();
        yield return new WaitForSecondsRealtime(introTiming);


        yield return TutorialStep(individualTooltips.triggerTooltip, triggerAudio, triggerAudioDone, triggerButton, pressedTrigger);
        yield return TutorialStep(individualTooltips.touchpadTooltip, touchpadAudio, touchpadAudioDone, touchpadButton, pressedTouchpad);
        yield return TutorialStep(individualTooltips.gripTooltip, gripAudio, gripAudioDone, gripButton, pressedGrip);
        yield return TutorialStep(individualTooltips.buttonOne, buttonOneAudio, buttonOneAudioDone, buttonOneButton, pressedButtonOne);
        yield return TutorialStep(individualTooltips.buttonTwo, buttonTwoAudio, buttonTwoAudioDone, buttonTwoButton, pressedButtonTwo);

    }

    IEnumerator TutorialStep(VRTK_ObjectTooltip tooltip, AudioSource tooltipAudio, Ref audioDone, VRTK_ControllerTooltips.TooltipButtons tooltipButton, Ref pressedTooltip)
    {
        tooltipAudio.Play();
        tooltips.ToggleTips(true, tooltipButton);
        yield return new WaitForSecondsRealtime(tooltipAudio.clip.length);
        audioDone.Value = true;
        yield return new WaitUntil(() => pressedTooltip.Value == true);
    }

    private void ChangeTooltipColorWhenPressed(VRTK_ObjectTooltip tooltip)
    {
        individualTooltips.ChangeContainerColor(tooltip, tipBackgroundColor_WhenPressed);
        individualTooltips.ChangeLineColor(tooltip, tipLineColor_WhenPressed);
        individualTooltips.ChangeFontColor(tooltip, tipTextColor_WhenPressed);
        tooltip.ResetTooltip();
    }

    private void ChangeTooltipColorAfterPressed(VRTK_ObjectTooltip tooltip)
    {
        individualTooltips.ChangeContainerColor(tooltip, tipBackgroundColor_AfterPressed);
        individualTooltips.ChangeLineColor(tooltip, tipLineColor_AfterPressed);
        individualTooltips.ChangeFontColor(tooltip, tipTextColor_AfterPressed);
    }
    
    private void DoTooltipPressed(bool audioDone, Ref pressedTooltip, VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
    {
        if (audioDone)
        {
            if (!pressedTooltip.Value)
            {
                ChangeTooltipColorWhenPressed(tooltip);
                pressedTooltip.Value = true;
                print(pressedTooltip);
                ChangeTooltipColorAfterPressed(tooltip);
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
        DoTooltipPressed(triggerAudioDone.Value, pressedTrigger, individualTooltips.triggerTooltip, triggerButton);
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(triggerAudioDone.Value, individualTooltips.triggerTooltip, triggerButton);
    }

    private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(buttonOneAudioDone.Value,  pressedButtonOne, individualTooltips.buttonOne, buttonOneButton);
    }

    private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(buttonOneAudioDone.Value, individualTooltips.buttonOne, buttonOneButton);
    }

    private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(buttonTwoAudioDone.Value, pressedButtonTwo, individualTooltips.buttonTwo, buttonTwoButton);
    }

    private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(buttonTwoAudioDone.Value, individualTooltips.buttonTwo, buttonTwoButton);
    }

    private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(gripAudioDone.Value, pressedGrip, individualTooltips.gripTooltip, gripButton);
    }

    private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(gripAudioDone.Value, individualTooltips.gripTooltip, gripButton);
    }

    private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipPressed(touchpadAudioDone.Value, pressedTouchpad, individualTooltips.touchpadTooltip, touchpadButton);
    }

    private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        DoTooltipReleased(touchpadAudioDone.Value, individualTooltips.touchpadTooltip, touchpadButton);
    }


    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        GetComponent<AutomaticTooltips>().enabled = true;
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        enabled = false;
    }

    private void SetupControllerEventListeners()
    {
        events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

        events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
        events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

        events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
        events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

        events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

        events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
    }
}
