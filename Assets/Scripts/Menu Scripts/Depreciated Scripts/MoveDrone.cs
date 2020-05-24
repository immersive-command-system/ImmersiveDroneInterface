namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    // This is a component of the basic drone gameObject

    public class MoveDrone : MonoBehaviour
    {
        public Material selectedPassedLine;
        public Material unselectedPassedLine;
        public float speed;
        public bool move;
        public GameObject targetWaypoint;
        public int currentWaypoint;
        public GameObject prevPoint;

        private int totalWaypoints;
        private LineRenderer line;
        private GameObject world;
        private Drone thisDrone;

        void Start()
        {
            thisDrone = gameObject.GetComponent<DroneProperties>().classPointer;
            currentWaypoint = 0;
            move = false;
            line = this.GetComponentInParent<LineRenderer>();
            world = GameObject.Find("World");
        }

        void Update()
        {
            totalWaypoints = thisDrone.waypoints.Count - 1;

            if (prevPoint != null)
            {
                DisplayPastPath();
            }

            SelectTarget();

            if (move)
            {
                Move();
            }
        }

        private void DisplayPastPath()
        {
            line.SetPosition(0, this.transform.position);
            if (thisDrone.waypoints.Count == 0)
            {
                line.SetPosition(1, this.transform.position);
            } else
            {
                line.SetPosition(1, prevPoint.transform.position);
            }
            line.startWidth = world.GetComponent<MapInteractions>().actualScale.y / 200;
            line.endWidth = world.GetComponent<MapInteractions>().actualScale.y / 200;
            if (thisDrone.selected)
            {
                line.material = selectedPassedLine;
            } else
            {
                line.material = unselectedPassedLine;
            }
        }

        private void SelectTarget()
        {
            if (thisDrone.waypoints.Count != 0)
            {
                if (totalWaypoints > currentWaypoint)
                {
                    targetWaypoint = ((Waypoint) thisDrone.waypoints[currentWaypoint + 1]).gameObjectPointer;
                    if (this.transform.position == targetWaypoint.transform.position)
                    {
                        prevPoint = (GameObject)thisDrone.waypoints[currentWaypoint + 1];
                        currentWaypoint++;
                    }
                }
                else if (totalWaypoints < currentWaypoint)
                {
                    //targetWaypoint = (GameObject)this.GetComponentInParent<SetWaypoint>().waypoints[totalWaypoints];
                    //prevPoint = (GameObject)this.GetComponentInParent<SetWaypoint>().waypoints[totalWaypoints];
                    //if (this.transform.position == targetWaypoint.transform.position)
                    //{
                    //    currentWaypoint = totalWaypoints;
                    //}
                    currentWaypoint = totalWaypoints;
                    prevPoint = (GameObject)thisDrone.waypoints[totalWaypoints];
                }
                else
                {
                    if (targetWaypoint != null && this.transform.position == targetWaypoint.transform.position)
                    {
                        targetWaypoint = null;
                    } else
                    {
                        targetWaypoint = prevPoint;
                    }
                }
            }
        }

        private void Move()
        {
            if (targetWaypoint != null)
            {
                float step = speed * Time.deltaTime * world.GetComponent<MapInteractions>().actualScale.x;
                this.transform.position = Vector3.MoveTowards(this.transform.position, targetWaypoint.transform.position, step);
            } else
            {
                move = false;
            }
        }
    }
}
