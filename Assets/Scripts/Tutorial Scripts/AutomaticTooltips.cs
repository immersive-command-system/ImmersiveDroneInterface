namespace ISAACS

using UnityEngine;
using VRTK;
using System.Collections;

public class AutomaticTooltips : MonoBehaviour
{

    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerEvents events;
    public AudioSource a, b, c, d, e, f, g, h, i, j, k;

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

        StartCoroutine(PlayAudio());
      //  events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
      //  events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

        tooltips.ToggleTips(false);
    }

    IEnumerator PlayAudio()
    {
        yield return new WaitForSecondsRealtime(4);
        a.Play();
        yield return new WaitForSeconds(a.clip.length);
        b.Play();
        yield return new WaitForSeconds(b.clip.length);
        c.Play();
        yield return new WaitForSeconds(c.clip.length);
        d.Play();
        yield return new WaitForSeconds(d.clip.length);
        e.Play();
        yield return new WaitForSeconds(e.clip.length);
        f.Play();
        yield return new WaitForSeconds(f.clip.length);
        g.Play();
        yield return new WaitForSeconds(g.clip.length);
        h.Play();
        yield return new WaitForSeconds(h.clip.length);
        i.Play();
        yield return new WaitForSeconds(i.clip.length);
        j.Play();
        yield return new WaitForSeconds(j.clip.length);
        k.Play();
        yield return new WaitForSeconds(k.clip.length);
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
        GetComponentInChildren<VRTK_ControllerTooltips>().enabled = !GetComponentInChildren<VRTK_ControllerTooltips>().enabled;
    }
    */


}
