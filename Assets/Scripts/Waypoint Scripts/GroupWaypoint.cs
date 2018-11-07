namespace ISAACS
{
    using UnityEngine;
    using VRTK;

    public class GroupWaypoint : GeneralWaypoint
    {
        private static int nextWaypointID = 0;
        private int waypointID;

        private string groupID;
        private GroupWaypoint previousWaypoint;
        private GroupWaypoint nextWaypoint;

        public GroupWaypoint(string groupID, Vector3 position, GroupWaypoint prevWaypoint, GroupWaypoint nextWaypoint = null)
        {
            this.groupID = groupID;
            DroneGroup group = WorldProperties.groupedDrones[groupID];

            this.waypointID = nextWaypointID++;

            GameObject baseObject = WorldProperties.worldObject.GetComponent<WorldProperties>().waypointBaseObject;
            gameObjectPointer = Object.Instantiate(baseObject, position, Quaternion.identity);
            gameObjectPointer.GetComponent<VRTK_InteractableObject>().ignoredColliders[0] = GameObject.Find("controller_right").GetComponent<SphereCollider>(); //Ignoring Collider from Controller so that WayPoint Zone is used
            gameObjectPointer.GetComponent<WaypointProperties>().classPointer = this; // Connect the gameObject back to the classObject
            gameObjectPointer.GetComponent<WaypointProperties>().SetLineWidthFactor(Mathf.Log(group.Count(), 2));
            gameObjectPointer.tag = "waypoint";
            gameObjectPointer.name = baseObject.name;
            gameObjectPointer.transform.localScale = WorldProperties.actualScale / 100 * Mathf.Log(group.Count(), 2);
            gameObjectPointer.transform.parent = WorldProperties.worldObject.transform;
            WorldProperties.AddClipShader(gameObjectPointer.transform);

            this.previousWaypoint = prevWaypoint;
            this.nextWaypoint = nextWaypoint;
        }

        public override void UpdateLineColliders()
        {
            gameObjectPointer.GetComponent<WaypointProperties>().UpdateLineCollider();
            if (nextWaypoint != null)
            {
                nextWaypoint.gameObjectPointer.GetComponent<WaypointProperties>().UpdateLineCollider();
            }
        }

        /// <summary>
        /// Get the position of this waypoint in Unity world coordinates.
        /// </summary>
        public override Vector3 GetPosition()
        {
            return gameObjectPointer.transform.position;
        }

        /// <summary>
        /// Get the ID of the group this waypoint is associated with.
        /// </summary>
        public string GetGroupID()
        {
            return this.groupID;
        }

        public override string ToString()
        {
            return "Group(" + groupID + ") waypoint " + waypointID;
        }

        /// <summary>
        /// Get the waypoint that comes before this on group itinerary.
        /// </summary>
        public override GeneralWaypoint GetPrevWaypoint()
        {
            return this.previousWaypoint;
        }

        public override void SetPrevWaypoint(GeneralWaypoint waypoint)
        {
            this.previousWaypoint = (GroupWaypoint)waypoint;
        }

        public override GeneralWaypoint GetNextWaypoint()
        {
            return this.nextWaypoint;
        }

        public override void SetNextWaypoint(GeneralWaypoint waypoint)
        {
            this.nextWaypoint = (GroupWaypoint)waypoint;
        }
    }
}