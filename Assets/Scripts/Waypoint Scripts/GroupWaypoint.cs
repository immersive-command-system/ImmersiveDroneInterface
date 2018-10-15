namespace ISAACS
{
    using UnityEngine;
    using VRTK;

    public class GroupWaypoint : GeneralWaypoint
    {
        private string groupID;

        public GroupWaypoint(string groupID, Vector3 position)
        {
            this.groupID = groupID;

            GameObject baseObject = (GameObject)WorldProperties.worldObject.GetComponent<WorldProperties>().waypointBaseObject;
            gameObjectPointer = Object.Instantiate(baseObject, position, Quaternion.identity);
            gameObjectPointer.GetComponent<VRTK_InteractableObject>().ignoredColliders[0] = GameObject.Find("controller_right").GetComponent<SphereCollider>(); //Ignoring Collider from Controller so that WayPoint Zone is used
            gameObjectPointer.tag = "waypoint";
            gameObjectPointer.name = baseObject.name;
            gameObjectPointer.transform.localScale = WorldProperties.actualScale / 100;
            gameObjectPointer.transform.parent = WorldProperties.worldObject.transform;
            WorldProperties.AddClipShader(gameObjectPointer.transform);
        }

        public override void UpdateLineColliders()
        {
            // TODO: Implement this.
        }

        /// <summary>
        /// Get the position of this waypoint in Unity world coordinates.
        /// </summary>
        public override Vector3 GetPosition()
        {
            return this.gameObjectPointer.transform.position;
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
            return "Waypoint of group: " + this.groupID;
        }
    }
}