namespace ISAACS
{
    using UnityEngine;
    using VRTK;

    //implement clicking on joystick to turn on labels afterwards

    public class Ref
    {
        private bool backing;
        public bool Value { get { return backing; } set { backing = value; } }
        public Ref() { backing = false; }
    }


    public class Tooltips : MonoBehaviour {

        private VRTK_ObjectTooltip[] individualTooltips;
        public VRTK_ObjectTooltip triggerTooltip, gripTooltip, touchpadTooltip, buttonOne, buttonTwo;

        private VRTK_ControllerTooltips tooltips;
        private VRTK_ControllerEvents events;

        private Ref pressedTrigger = new Ref(), pressedGrip = new Ref(), pressedTouchpad = new Ref(), pressedButtonOne = new Ref(), pressedButtonTwo = new Ref();
        private Ref triggerAudioDone = new Ref(), gripAudioDone = new Ref(), touchpadAudioDone = new Ref(), buttonOneAudioDone = new Ref(), buttonTwoAudioDone = new Ref();

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
            individualTooltips = GetComponentsInChildren<VRTK_ObjectTooltip>(true);
            events = GetComponent<VRTK_ControllerEvents>();
            tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
            SetupControllerEventListeners();
            InitializeIndividualTooltips();
        }

        void InitializeIndividualTooltips()
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
                        touchpadTooltip = tooltip;
                        break;
                    case "buttonone":
                        buttonOne = tooltip;
                        break;
                    case "buttontwo":
                        buttonTwo = tooltip;
                        break;
                }
            }
        }

        public void ChangeContainerColor(VRTK_ObjectTooltip tooltip, Color newColor)
        {
            tooltip.containerColor = newColor;
        }

        public void ChangeFontColor(VRTK_ObjectTooltip tooltip, Color newColor)
        {
            tooltip.fontColor = newColor;
        }

        public void ChangeLineColor(VRTK_ObjectTooltip tooltip, Color newColor)
        {
            tooltip.lineColor = newColor;
        }

        private void ChangeTooltipColorWhenPressed(VRTK_ObjectTooltip tooltip)
        {
            ChangeContainerColor(tooltip, tipBackgroundColor_WhenPressed);
            ChangeLineColor(tooltip, tipLineColor_WhenPressed);
            ChangeFontColor(tooltip, tipTextColor_WhenPressed);
            tooltip.ResetTooltip();
        }

        private void ChangeTooltipColorAfterPressed(VRTK_ObjectTooltip tooltip)
        {
            ChangeContainerColor(tooltip, tipBackgroundColor_AfterPressed);
            ChangeLineColor(tooltip, tipLineColor_AfterPressed);
            ChangeFontColor(tooltip, tipTextColor_AfterPressed);
        }

        private void DoTooltipPressed(Ref audioDone, Ref pressedTooltip, VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
        { 
            if(tooltipButton == VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip)
            {
                if (Tutorial.currentTutorialState == Tutorial.TutorialState.MOVINGMAP)
                {
                
                } else
                {

                }
                pressedTooltip.Value = true;
                if (currentTutorialState == 0)
                {
                    ChangeTooltipColorWhenPressed(tooltip);
                }
                else
                {
                    ChangeTooltipColorAfterPressed(tooltip);
                }
            }

            if ()
            {

            }
        }
       
    
        private void DoTooltipReleased(VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
        {
                tooltips.ToggleTips(false, tooltipButton);
                tooltip.ResetTooltip();
        }
    
        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipPressed(triggerAudioDone, pressedTrigger, triggerTooltip, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(triggerTooltip, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipPressed(buttonOne, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(buttonOne, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipPressed(buttonTwo, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(buttonTwo, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipPressed(gripTooltip, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(gripTooltip, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
        }

        private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipPressed(touchpadTooltip, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(touchpadTooltip, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
          //  GetComponent<AutomaticTooltips>().enabled = true;
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
          //  enabled = false;
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
}