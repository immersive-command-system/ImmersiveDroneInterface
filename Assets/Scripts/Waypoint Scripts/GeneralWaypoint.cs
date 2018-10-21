namespace ISAACS
{
    using UnityEngine;

    public abstract class GeneralWaypoint
    {
        public GameObject gameObjectPointer;

        public abstract Vector3 GetPosition();

        public abstract void UpdateLineColliders();

        public abstract GeneralWaypoint GetPrevWaypoint();

        public abstract void SetPrevWaypoint(GeneralWaypoint waypoint);

        /// <summary>
        /// Assign a new position for this waypoint in Unity world coordinates
        /// </summary>
        /// <param name="newPosition">The new position to assign this waypoint.</param>
        public void AssignPosition(Vector3 newPosition)
        {
            this.gameObjectPointer.transform.position = newPosition;
            gameObjectPointer.GetComponent<WaypointProperties>().UpdateGroundpointLine();
        }
    }
}