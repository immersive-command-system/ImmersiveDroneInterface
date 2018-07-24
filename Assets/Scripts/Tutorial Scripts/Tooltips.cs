namespace ISAACS
{
    using UnityEngine;
    using VRTK;

    /// <summary>
    /// This adds a collection of Object Tooltips to the Controller that give information on what the main controller buttons may do. 
    /// </summary>
    /// <remarks>
    /// It needs to be added to the relevant alias controller GameObjects (i.e. RightController, LeftController), and the controller GameObjects should be children of the same parent GameObject (i.e. "Controllers)
    /// </remarks>

    public class Tooltips : MonoBehaviour {

        private VRTK_ObjectTooltip[] individualTooltips; //this contains the possible tooltips and is used for initialization
        public VRTK_ObjectTooltip triggerTooltip, gripTooltip, touchpadTooltip, buttonOne, buttonTwo; //possible tooltips

        private VRTK_ControllerTooltips tooltips; //need to access VRTK tooltip functions
        private VRTK_ControllerEvents events; //for setting event listeners

        private Tutorial tutorial; //for getting the color of the tooltip during the tutorial step

        /// <summary>
        /// The start method initializes all necessary variables and sets up event listeners for the tooltip buttons and the place point
        /// </summary>
        void Start()
        {
            individualTooltips = GetComponentsInChildren<VRTK_ObjectTooltip>(true);
            events = GetComponent<VRTK_ControllerEvents>();
            tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
            tutorial = GetComponentInParent<Tutorial>();
            SetupControllerEventListeners();
            InitializeIndividualTooltips();
        }

        /// <summary>
        /// This initializes the tooltip objects
        /// </summary>
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

        /// <summary>
        /// This function is used as a helper to get a tooltip object given a button of the TooltipButtons enum type
        /// </summary>
        /// <param name="button">this is the button that we want the corresponding tooltip of</param>
        /// <returns>This is the corresponding tooltip object</returns>
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

        /// <summary>
        /// This function is used as a helper to get a TooltipButtons enum item given a tooltip object
        /// </summary>
        /// <param name="tooltip">This is the tooltip that we want the corresponding enum item of</param>
        /// <returns>This is the corresponding enum item</returns>
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

        /// <summary>
        /// This function changes the colour to use for the background container of the tooltip.
        /// </summary>
        /// <param name="tooltip">The tooltip that you want to change the color of</param>
        /// <param name="newColor">The color to be changed to</param>
        public void ChangeContainerColor(VRTK_ObjectTooltip tooltip, Color newColor)
        {
            tooltip.containerColor = newColor;
        }

        /// <summary>
        /// This function changes the colour to use for the text on the tooltip.
        /// </summary>
        /// <param name="tooltip">The tooltip that you want to change the color of</param>
        /// <param name="newColor">The color to be changed to</param>
        public void ChangeFontColor(VRTK_ObjectTooltip tooltip, Color newColor)
        {
            tooltip.fontColor = newColor;
        }

        /// <summary>
        /// The function changes colour to use for the line drawn between the tooltip and the destination transform.
        /// </summary>
        /// <param name="tooltip">The tooltip that you want to change the color of</param>
        /// <param name="newColor">The color to be changed to</param>
        public void ChangeLineColor(VRTK_ObjectTooltip tooltip, Color newColor)
        {
            tooltip.lineColor = newColor;
        }

        /// <summary>
        /// This function determines the behavior of the tooltip when the corresponding button is pressed
        /// </summary>
        /// <param name="tooltip">The tooltip which should have its behavior changed</param>
        /// <param name="tooltipButton">The button that corresponds to the tooltip</param>
        private void DoTooltipPressed(VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
        {
            tooltips.ToggleTips(true, tooltipButton);
        }


        /// <summary>
        /// This function determines the behavior of the tooltip when the corresponding button is released
        /// </summary>
        /// <param name="tooltip">The tooltip which should have its behavior changed</param>
        /// <param name="tooltipButton">The button that corresponds to the tooltip</param>
        private void DoTooltipReleased(VRTK_ObjectTooltip tooltip, VRTK_ControllerTooltips.TooltipButtons tooltipButton)
        {
            Debug.Log("1" + tooltip.containerColor);
            Debug.Log("2" + tutorial.tipBackgroundColor_DuringTutorialStep);
           if (tooltip.containerColor != tutorial.tipBackgroundColor_DuringTutorialStep)
            {
                tooltips.ToggleTips(false, tooltipButton);
            }
            
            /*
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
            }*/
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
          
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
          
        }
        */

        /// <summary>
        /// This function sets up the event handlers for when controller buttons are pressed and released
        /// </summary>
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