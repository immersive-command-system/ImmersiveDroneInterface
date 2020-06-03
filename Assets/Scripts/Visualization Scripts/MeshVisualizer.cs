using System;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.voxblox_msgs;

public class MeshVisualizer : MonoBehaviour
{
    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.</value>
    public bool flipYZ = false;

    private GameObject meshParent;

    private bool hasChanged = false;

    private Dictionary<Int64[], List<float[,]>> mesh_dict;    // Use this for initialization
    void Start()
    {
        mesh_dict = new Dictionary<long[], List<float[,]>>();
        meshParent = new GameObject("Initial Mesh");
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
    /// <param name="meshMsg">ROSBridge Voxblox Mesh Message</param>
    public void SetMesh(MeshMsg meshMsg)
    {
        Debug.Log("Setting New Mesh");
        float scale_factor = meshMsg.GetBlockEdgeLength();

        MeshBlockMsg[] mesh_blocks = meshMsg.GetMeshBlocks();
        for (int i = 0; i < mesh_blocks.Length; i++)
        {
            Int64[] index = mesh_blocks[i].GetIndex();

            UInt16[] x = mesh_blocks[i].GetX();
            float[] pos_x = new float[x.Length];
            for (int j = 0; j < pos_x.Length; j++)
            {
                pos_x[i] = (float)x[i] / 32768.0f + index[0] * scale_factor;
            }

            UInt16[] y = mesh_blocks[i].GetY();
            float[] pos_y = new float[y.Length];
            for (int j = 0; j < pos_x.Length; j++)
            {
                pos_y[i] = (float)y[i] / 32768.0f + index[1] * scale_factor;
            }

            UInt16[] z = mesh_blocks[i].GetZ();
            float[] pos_z = new float[z.Length];
            for (int j = 0; j < pos_z.Length; j++)
            {
                pos_z[i] = (float)z[i] / 32768.0f + index[2] * scale_factor;
            }

            byte[] r = mesh_blocks[i].GetR();
            byte[] g = mesh_blocks[i].GetG();
            byte[] b = mesh_blocks[i].GetB();

            float[,] pos_list = new float[3, pos_x.Length];
            float[,] color_list = new float[3, r.Length];
            for (int j = 0; j < pos_x.Length; j++)
            {
                pos_list[0, j] = pos_x[j];
                pos_list[1, j] = pos_y[j];
                pos_list[2, j] = pos_z[j];
            }
            for (int j = 0; j < r.Length; j++)
            {
                color_list[0, j] = (float)(r[j]);
                color_list[1, j] = (float)(g[j]);
                color_list[2, j] = (float)(b[j]);
            }

            List<float[,]> data_list = new List<float[,]>();
            data_list.Add(pos_list);
            data_list.Add(color_list);
            mesh_dict.Add(index, data_list);
        }

        List<Vector3> newVertices = new List<Vector3>();
        // Also not sure what to do with all the newColors...
        List<Color> newColors = new List<Color>();
        foreach (KeyValuePair<Int64[], List<float[,]>> entry in mesh_dict)
        {
            float[,] pos_list = entry.Value[0];
            Vector3[] verts = new Vector3[pos_list.GetLength(0)];

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] = new Vector3(pos_list[0, i], pos_list[1, i], pos_list[2, i]);
            }

            newVertices.AddRange(verts);

            float[,] color_list = entry.Value[1];
            Color[] colors = new Color[color_list.GetLength(0)];

            for (int i = 0; i < verts.Length; i++)
            {
                colors[i] = new Color(color_list[0, i], color_list[1, i], color_list[2, i], 0.2f);
            }

            newColors.AddRange(colors);
        }

        int[] newTriangles = new int[newVertices.Count / 3 * 3];
        for (int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] = i;
        }

        Destroy(meshParent);
        // Insert timestamp here maybe?
        meshParent = new GameObject("Mesh");

        MeshRenderer meshRenderer = meshParent.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        // TODO
        Mesh mesh = new Mesh();
        MeshFilter meshFilter = meshParent.AddComponent<MeshFilter>();
        mesh.vertices = newVertices.ToArray();
        // ?
        //mesh.uv = newUV;
        // Also not sure if this is correct either... Python and Unity seem to disagree on this point.
        mesh.triangles = newTriangles;

        // colors may not be the same lengths as vertices. Unity demands that it be the same as the vertices.
        mesh.colors = newColors.ToArray();
        meshFilter.mesh = mesh;

        hasChanged = true;
    }
}

 