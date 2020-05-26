using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PointCloud;
using ISAACS;

public class PointCloudVisualizer : MonoBehaviour
{
    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.</value>
    public bool flipYZ = false;


    Color red = new Color(255, 0, 0, 1);
    Color orange = new Color(255, 130, 0, 1);
    Color yellow = new Color(255, 255, 0, 1);
    Color green = new Color(0, 255, 0, 1);
    Color blue = new Color(0, 0, 255, 1);
    Color light_blue = new Color(52, 174, 235, 1);
    Color white = new Color(255, 255, 255, 1);


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
    public void SetPointCloud(PointCloud<PointXYZRGBAIntensity> newCloud)
    {
        Destroy(cloudParent);
        // Insert timestamp here maybe?
        cloudParent = new GameObject("PointCloud");
        foreach (PointXYZRGBAIntensity point in newCloud.Points)
        {
            GameObject childPoint = Instantiate(pointObject);
            childPoint.transform.position = (flipYZ) ? new Vector3(point.X, point.Z, point.Y) : new Vector3(point.X, point.Y, point.Z);
            childPoint.transform.parent = cloudParent.transform;
            
            MeshRenderer pRenderer = childPoint.GetComponent<MeshRenderer>();
            Material pMaterial = new Material(Shader.Find("Unlit/Color"));
            Color color = new Color(point.R, point.G, point.B, point.A);


            // Peru: 5/26/20 : New color scheme

            int pointCloudLevel = WorldProperties.worldObject.GetComponent<ROSDroneConnection>().pointCloudLevel;
            
            if (pointCloudLevel == 5)
            {
                color = red;
            }
            else if (pointCloudLevel == 4)
            {
                color = orange;
            }
            else if (pointCloudLevel == 3)
            {
                color = orange;
            }
            else if (pointCloudLevel == 2)
            {
                color = orange;
            }
            else if (pointCloudLevel == 1)
            {
                color = orange;
            }
            else if (pointCloudLevel == 0)
            {
                color = orange;
            }
            else
            {
                color = white;
            }


            pMaterial.color = color;
            pRenderer.material = pMaterial;
            // TODO do something with the intensity
        }
        hasChanged = true;
    }

    /// <summary>
    /// Replace the current point cloud with a new point cloud.
    /// </summary>
    /// <param name="newCloud"></param>
    public void SetPointCloud(PointCloud<PointXYZRGBA> newCloud)
    {
        Destroy(cloudParent);
        // Insert timestamp here maybe?
        cloudParent = new GameObject("PointCloud");
        foreach (PointXYZRGBA point in newCloud.Points)
        {
            GameObject childPoint = Instantiate(pointObject);
            childPoint.transform.position = (flipYZ) ? new Vector3(point.X, point.Z, point.Y) : new Vector3(point.X, point.Y, point.Z);
            childPoint.transform.parent = cloudParent.transform;
            
            Color color = new Color(point.R, point.G, point.B, point.A);
            MeshRenderer pRenderer = childPoint.GetComponent<MeshRenderer>();
            Material pMaterial = new Material(Shader.Find("Unlit/Color"));
            pMaterial.color = color;
            pRenderer.material = pMaterial;
        }
        hasChanged = true;
    }


    /// <summary>
    /// Replace the current point cloud with a new point cloud.
    /// </summary>
    /// <param name="newCloud"></param>
    public void SetPointCloud(PointCloud<PointXYZRGB> newCloud)
    {
        Destroy(cloudParent);
        // Insert timestamp here maybe?
        cloudParent = new GameObject("PointCloud");
        foreach (PointXYZRGB point in newCloud.Points)
        {
            GameObject childPoint = Instantiate(pointObject);
            childPoint.transform.position = (flipYZ) ? new Vector3(point.X, point.Z, point.Y) : new Vector3(point.X, point.Y, point.Z);
            childPoint.transform.parent = cloudParent.transform;
            
            Color color = new Color(point.R, point.G, point.B, 1);
            MeshRenderer pRenderer = childPoint.GetComponent<MeshRenderer>();
            Material pMaterial = new Material(Shader.Find("Unlit/Color"));
            pMaterial.color = color;
            pRenderer.material = pMaterial;
        }
        hasChanged = true;
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

            MeshRenderer pRenderer = childPoint.GetComponent<MeshRenderer>();
            Material pMaterial = new Material(Shader.Find("Unlit/Color"));
            // TODO: Intensities are porbably between 0 and 1
            Color red = new Color(255, 0, 0, 1);
            Color orange = new Color(255, 130, 0, 1);
            Color yellow = new Color(255, 255, 0, 1);
            Color green = new Color(0, 255, 0, 1);
            Color blue = new Color(0, 0, 255, 1);

            if (point.intensity == 1)
            {
                pMaterial.color = red;
            }
            else if (point.intensity == 0)
            {
                pMaterial.color = orange;
            }
            else if (point.intensity == 0)
            {
                pMaterial.color = yellow;
            }
            else if (point.intensity == 0)
            {
                pMaterial.color = green;
            }
            else if (point.intensity == 0)
            {
                pMaterial.color = blue;
            }
            pRenderer.material = pMaterial;
        }
        hasChanged = true;
    }

}

