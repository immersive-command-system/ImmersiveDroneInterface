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

    private Dictionary<Int64[], MeshFilter> mesh_dict;    // Use this for initialization
    void Start()
    {
        mesh_dict = new Dictionary<long[], MeshFilter>();
        meshParent = new GameObject("Mesh");
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
        Debug.Log(mesh_blocks.Length);
        for (int i = 0; i < mesh_blocks.Length; i++)
        {
            Int64[] index = mesh_blocks[i].GetIndex();

            List<Vector3> newVertices = new List<Vector3>();
            // Also not sure what to do with all the newColors...
            List<Color> newColors = new List<Color>();
        
            UInt16[] x = mesh_blocks[i].GetX();
            UInt16[] y = mesh_blocks[i].GetY();
            UInt16[] z = mesh_blocks[i].GetZ();
            for (int j = 0; j < x.Length; j++)
            {
                float zv = ((float)z[j] / 32768.0f + index[2]) * scale_factor;
                float xv = ((float)x[j] / 32768.0f + index[0]) * scale_factor;
                float yv = ((float)y[j] / 32768.0f + index[1]) * scale_factor;
                if (flipYZ)
                {
                    newVertices.Add(new Vector3(xv, zv, yv));
                } 
                else
                {
                    newVertices.Add(new Vector3(xv, yv, zv));
                }
            }
            byte[] r = mesh_blocks[i].GetR();
            byte[] g = mesh_blocks[i].GetG();
            byte[] b = mesh_blocks[i].GetB();

            for (int j = 0; j < r.Length; j++)
            {
                if (j == 0)
                {
                    Debug.Log("R: " + r[j] + " G: " + g[j] + " B: " + b[j]);
                }
                newColors.Add(new Color32(r[j], g[j], b[j], 51));
            }

            Debug.Log("Color Length:" + r.Length);
            //    mesh_dict.Add(index, data_list);

            int[] newTriangles = new int[newVertices.Count / 3 * 3];
            for (int j = 0; j < newTriangles.Length; j++)
            {
                newTriangles[j] = j;
            }

            // TODO
            Mesh mesh = new Mesh();
            if (!mesh_dict.ContainsKey(index))
            {
                GameObject meshObject = new GameObject(index.ToString());
                meshObject.transform.parent = meshParent.transform;
                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(Shader.Find("Particles/Standard Surface"));
                mesh_dict.Add(index, meshFilter);
            }
            mesh.vertices = newVertices.ToArray();
            // ?
            //mesh.uv = newUV;
            // Also not sure if this is correct either... Python and Unity seem to disagree on this point.
            mesh.triangles = newTriangles;

            // colors may not be the same lengths as vertices. Unity demands that it be the same as the vertices.
            mesh.colors = newColors.ToArray();
            mesh_dict[index].mesh = mesh;
        }
        hasChanged = true;
    }
}

 