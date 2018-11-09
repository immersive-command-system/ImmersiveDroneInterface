using UnityEngine;
using System.Collections;

public class TurnOnDepthBuffer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
}