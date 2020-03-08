using ISAACS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHardcodedWaypoint : MonoBehaviour {
    
    public float waypointLatitude;
    public float waypointLongitude;
    public float waypointAltitude;

    public float initialLat;

    public GameObject hardcodedWaypointSphere;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (WorldProperties.droneInitialPositionSet)
        {
           Vector3 changePos = new Vector3(
                   ((WorldProperties.LatDiffMeters(WorldProperties.droneHomeLat, waypointLatitude)) / WorldProperties.Unity_X_To_Lat_Scale),
                   ((waypointAltitude /*- WorldProperties.droneHomeAlt*/) / WorldProperties.Unity_Y_To_Alt_Scale),
                   ((WorldProperties.LongDiffMeters(WorldProperties.droneHomeLong, waypointLongitude, waypointLatitude) / WorldProperties.Unity_Z_To_Long_Scale))
                 );
            initialLat = WorldProperties.droneHomeLat;
            this.transform.localPosition = changePos;
        }
		
	}
}
