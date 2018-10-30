namespace ISAACS
{
    using UnityEngine;

    /// <summary>
    /// Superclass to Waypoint and GroupWaypoint and prototypes some common waypoint functions.
    /// </summary>
    public abstract class GeneralWaypoint
    {
        public GameObject gameObjectPointer;

        private bool isInteractable = true;
        private bool isVisible = true;

        public abstract Vector3 GetPosition();

        public abstract void UpdateLineColliders();

        public abstract GeneralWaypoint GetPrevWaypoint();

        public abstract void SetPrevWaypoint(GeneralWaypoint waypoint);

        public abstract GeneralWaypoint GetNextWaypoint();

        public abstract void SetNextWaypoint(GeneralWaypoint waypoint);

        /// <summary>
        /// Assign a new position for this waypoint in Unity world coordinates
        /// </summary>
        /// <param name="newPosition">The new position to assign this waypoint.</param>
        public void AssignPosition(Vector3 newPosition)
        {
            this.gameObjectPointer.transform.position = newPosition;
            WaypointProperties properties = gameObjectPointer.GetComponent<WaypointProperties>();
            properties.UpdateGroundpointLine();
        }

        public bool IsInteractable()
        {
            return this.isInteractable;
        }

        public void SetInteractable(bool isInteractable)
        {
            this.isInteractable = isInteractable;
            if (this.isInteractable && !isInteractable)
            {
                WaypointProperties.controller_right.GetComponent<ControllerInteractions>().OnTriggerExit(gameObjectPointer.GetComponent<SphereCollider>());
            }
            gameObjectPointer.GetComponent<SphereCollider>().enabled = isInteractable;
        }

        public bool IsVisible()
        {
            return this.isVisible;
        }

        public void SetVisible(bool isVisible)
        {
            gameObjectPointer.GetComponent<MeshRenderer>().enabled = isVisible;
            this.isVisible = isVisible;
        }
    }
}