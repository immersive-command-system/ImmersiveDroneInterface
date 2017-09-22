namespace VRTK
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class WaypointAdder : MonoBehaviour
    {
        public void OnClick()
        {
            GameObject temp = GameObject.Find("RightController");
            temp.GetComponent<VRTK_StraightPointerRenderer>().OnClick();
        }
    }
}