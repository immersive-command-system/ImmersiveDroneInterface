using System;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.voxblox_msgs;
using ROSBridgeLib.rntools;

public class PCFaceVisualizer : MonoBehaviour
{
    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.</value>
    public bool flipYZ = false;

    /// <summary>
    /// Object that holds all the individual mesh blocks.
    /// </summary>
    private GameObject meshParent;

    private bool hasChanged = false;

    /// <summary>
    /// Number of faces required for a block to be deemed worthy of being rendered.
    /// </summary>
    public int faceThreshold = 25;
    /// <summary>
    /// Time in seconds between accepting updates of a block.
    /// </summary>
    public float updateInterval = 30.0f;
    /// <summary>
    /// Maximum distance from the drone to override update time delay. Note that this is total taxi distance x+y+z not crow distance x^2+y^2+z^2
    /// </summary>
    public float distThreshold = 10.0f;

    void Start()
    {
        meshParent = new GameObject("PCFace Mesh");
        MeshFilter meshFilter = meshParent.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshParent.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
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
    /// Update the mesh with the new mesh.
    /// </summary>
    /// <param name="meshMsg">ROSBridge PCFace Mesh Message</param>
    public void SetMesh(PCFaceMsg meshMsg)
    {
        Debug.Log("Setting New PCFace Mesh");

        List<Vector3> newVertices = new List<Vector3>();
        // Also not sure what to do with all the newColors...
        List<Color> newColors = new List<Color>();

        float[] x = meshMsg.Vert_x;
        float[] y = meshMsg.Vert_y;
        float[] z = meshMsg.Vert_z;
        for (int j = 0; j < x.Length; j++)
        {
            if (flipYZ)
            {
                newVertices.Add(new Vector3(x[j], z[j], y[j]));
            } 
            else
            {
                newVertices.Add(new Vector3(x[j], y[j], z[j]));
            }
        }
        byte[] r = meshMsg.Color_r;
        byte[] g = meshMsg.Color_g;
        byte[] b = meshMsg.Color_b;
        byte[] a = meshMsg.Color_a;

        for (int j = 0; j < r.Length; j++)
        {
            newColors.Add(new Color32(r[j], g[j], b[j], a[j]));
        }

        int[] newTriangles = new int[newVertices.Count / 3 * 3];
        for (int j = 0; j < newTriangles.Length; j++)
        {
            newTriangles[j] = j;
        }

        // DO something with Face_0, face_1, face_2

        Debug.Log("Face_0 Length: " + meshMsg.Face_0.Length + "Face_1 Length: " + meshMsg.Face_1.Length + "Face_2 Length: " + meshMsg.Face_2.Length);
        Debug.Log("Num Verticies: " + newVertices.Count);
        Debug.Log("Num Colors: " + newColors.Count);

        Mesh mesh = new Mesh(); 
        mesh.vertices = newVertices.ToArray();
        // ?
        //mesh.uv = newUV;
        // Also not sure if this is correct either... Python and Unity seem to disagree on this point.
        mesh.triangles = newTriangles;

        // colors may not be the same lengths as vertices. Unity demands that it be the same as the vertices.
        mesh.colors = newColors.ToArray();
        meshParent.GetComponent<MeshFilter>().mesh = mesh;
        hasChanged = true;
    }
}