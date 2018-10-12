namespace ISAACS
{
    using UnityEngine;

    public abstract class GeneralWaypoint
    {
        public abstract Vector3 GetPosition();

        public abstract void UpdateLineColliders();

        public abstract void AssignPosition(Vector3 newPosition);
    }
}