using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PointCloud;

public class PointCloudVisualizer : MonoBehaviour
{
    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.</value>
    public bool flipYZ = false;

    /// <summary>
    /// Object to use to represent points in the point cloud.
    /// </summary>
    public GameObject pointObject;

    /// <summary>
    /// scale of the pointObject.
    /// </summary>
    public float size = 0.1f;

    private GameObject cloudParent;

    private bool hasChanged = false;

    // Use this for initialization
    void Start()
    {
        cloudParent = new GameObject("Initial");
        // configure the pointObject here.
        pointObject.transform.localScale = new Vector3(size, size, size);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (hasChanged)
        {
            // Do stuff maybe?
        }
    }


    /// <summary>
    /// Replace the current point cloud with a new point cloud.
    /// </summary>
    /// <param name="newCloud"></param>
    public void SetPointCloud(PointCloud<PointXYZIntensity> newCloud)
    {
        Destroy(cloudParent);
        // Insert timestamp here maybe?
        cloudParent = new GameObject("PointCloud");
        foreach (PointXYZIntensity point in newCloud.Points)
        {
            GameObject childPoint = Instantiate(pointObject);
            childPoint.transform.position = (flipYZ) ? new Vector3(point.X, point.Z, point.Y) : new Vector3(point.X, point.Y, point.Z);
            childPoint.transform.parent = cloudParent.transform;
            // TODO color by intensity
            // Use point.intensity
            
        }

        hasChanged = true;
    }

}

