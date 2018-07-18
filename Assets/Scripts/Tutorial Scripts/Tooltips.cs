namespace ISAACS
{
    using UnityEngine;
    using VRTK;

    //implement clicking on joystick to turn on labels afterwards

  /*  public class Ref
    {
        private bool backing;
        public bool Value { get { return backing; } set { backing = value; } }
        public Ref() { backing = false; }
    }*/


    public class Tooltips : MonoBehaviour {

        private VRTK_ObjectTooltip[] individualTooltips;
        public VRTK_ObjectTooltip triggerTooltip, gripTooltip, touchpadTooltip, buttonOne, buttonTwo;

        private VRTK_ControllerTooltips tooltips;
        private VRTK_ControllerEvents events;

      //  private Ref pressedTrigger = new Ref(), pressedGrip = new Ref(), pressedTouchpad = new Ref(), pressedButtonOne = new Ref(), pressedButtonTwo = new Ref();
     //   private Ref triggerAudioDone = new Ref(), gripAudioDone = new Ref(), touchpadAudioDone = new Ref(), buttonOneAudioDone = new Ref(), buttonTwoAudioDone = new Ref();


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

        public VRTK_ObjectTooltip ChangeButtonToTooltip(VRTK_ControllerTooltips.TooltipButtons button)
        {
            switch(button)
            {
                case VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip:
                    return triggerTooltip;
                case VRTK_ControllerTooltips.TooltipButtons.GripTooltip:
                    return gripTooltip;
                case VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip:
                    return touchpadTooltip;
                case VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip:
                    return buttonOne;
                case VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip:
                    return buttonTwo;
            }
            throw new System.ArgumentException("Parameter must be a valid tooltip button");
        }

        public VRTK_ControllerTooltips.TooltipButtons ChangeTooltipToButton(VRTK_ObjectTooltip tooltip)
        {
            if (tooltip.Equals(triggerTooltip))
            {
                return VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip;
            } else if (tooltip.Equals(gripTooltip))
            {
                return VRTK_ControllerTooltips.TooltipButtons.GripTooltip;
            } else if (tooltip.Equals(touchpadTooltip))
            {
                return VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip;
            } else if (tooltip.Equals(buttonOne))
            {
                return VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip;
            } else if (tooltip.Equals(buttonTwo))
            {
                return VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip;
            }
            throw new System.ArgumentException("Parameter must be a valid tooltip");

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

        private void DoTooltipPressed(VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
        {
            /*switch (tooltipButton)
            {
                case VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip:
                    if (gameObject.name == "LeftController" || (Tutorial.currentTutorialState != Tutorial.TutorialState.GRABZONE 
                            && Tutorial.currentTutorialState != Tutorial.TutorialState.PRIMARYPLACEMENT 
                            && Tutorial.currentTutorialState != Tutorial.TutorialState.SECONDARYPLACEMENT))
                    {
                        tooltips.ToggleTips(true, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.GripTooltip:
                    if (Tutorial.currentTutorialState != Tutorial.TutorialState.MOVINGMAP)
                    {
                        tooltips.ToggleTips(true, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip:
                    if (Tutorial.currentTutorialState != Tutorial.TutorialState.MOVINGMAP)
                    {
                        tooltips.ToggleTips(true, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip:
                    if (Tutorial.currentTutorialState != Tutorial.TutorialState.MOVINGMAP)
                    {
                        tooltips.ToggleTips(true, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip:
                    if (Tutorial.currentTutorialState != Tutorial.TutorialState.MOVINGMAP)
                    {
                        tooltips.ToggleTips(true, tooltipButton);
                    }
                    break;
            }*/
            tooltips.ToggleTips(true, tooltipButton);

        }
    
        private void DoTooltipReleased(VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
        {
            switch (tooltipButton)
            {
                case VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip:
                    if (gameObject.name == "LeftController" || (Tutorial.currentTutorialState != Tutorial.TutorialState.GRABZONE
                            && Tutorial.currentTutorialState != Tutorial.TutorialState.PRIMARYPLACEMENT
                            && Tutorial.currentTutorialState != Tutorial.TutorialState.SECONDARYPLACEMENT))
                    {
                        tooltips.ToggleTips(false, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.GripTooltip:
                    if ((gameObject.name == "LeftController" && Tutorial.currentTutorialState != Tutorial.TutorialState.SCALINGMAP)
                        || (Tutorial.currentTutorialState != Tutorial.TutorialState.SELECTIONPOINTER
                        && Tutorial.currentTutorialState != Tutorial.TutorialState.SECONDARYPLACEMENT
                        && Tutorial.currentTutorialState != Tutorial.TutorialState.SCALINGMAP))
                    {
                        tooltips.ToggleTips(false, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip:
                    if ((gameObject.name == "LeftController" && Tutorial.currentTutorialState != Tutorial.TutorialState.MOVINGMAP)
                        || (gameObject.name == "RightController" && Tutorial.currentTutorialState != Tutorial.TutorialState.ROTATINGMAP))
                    {
                        tooltips.ToggleTips(false, tooltipButton);
                    }
                    break;
                case VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip:
                    if (gameObject.name == "LeftController" 
                        || (gameObject.name == "RightController" && Tutorial.currentTutorialState != Tutorial.TutorialState.UNDOANDDELETE))
                    {
                        tooltips.ToggleTips(false, tooltipButton);
                    }
                    break;
            }
        }
    
        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipPressed(triggerTooltip, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(triggerTooltip, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        }
        /*
        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
           // DoTooltipPressed(buttonOne, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            DoTooltipReleased(buttonOne, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
        }
        */
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
        /*
        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
          //  GetComponent<AutomaticTooltips>().enabled = true;
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
          //  enabled = false;
        }
        */
        private void SetupControllerEventListeners()
        {
            events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
            events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

       //     events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
       //     events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

            events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
            events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

            events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
            events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        //    events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        //    events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

            events.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
            events.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
        }
    }
}