namespace ISAACS
{
    using UnityEngine;

    /** A class attached to a line object.
     *  Stores a reference to the waypoint it originally would be attached to.
     */
    public class LineProperties : MonoBehaviour
    {
        public GeneralWaypoint originWaypoint;
    }
}
