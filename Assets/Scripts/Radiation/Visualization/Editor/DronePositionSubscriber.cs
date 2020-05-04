using UnityEngine;
using UnityEditor;

/// <summary>
/// This file takes care of parsing messages about the Drone position that are received by the DataServer.
/// The data currently comes in a format of "x, y, z".
/// </summary>
public class DronePositionSubscriber : MonoBehaviour, DataServer.DataSubscriber
{
    /// <value> Attach DataServer object. If nonexistant, create an empty GameObject and attach the script `DataServer.cs`.</value>
    public DataServer server;

    /// <value> Setting this to true will give a horizontal view of the data - This should be equal throughout all subscribers.</value>
    public bool flipYZ = false;

    /// <value> Set to true to see the trail of the drone.</value>
    public bool renderTrail = true;

    /// <value> The most recent position of the drone. </value>
    private Vector3 newPosition;
    /// <value> A flag that indicates whether there was new position since the last time it was reset to false.</value>
    private bool positionDidUpdate = false;

    private bool finished = false;

    // Start is called before the first frame update
    void Start()
    {
        // This is how we show the drone's path history.
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null && renderTrail == true)
        {
            trail = gameObject.AddComponent<TrailRenderer>() as TrailRenderer;
            trail.widthMultiplier = 0.25f;
            trail.time = 10000;
            trail.material.color = new Color(0.0f, 1.0f, 1.0f, 0.5f);
        }
        else if (trail != null && renderTrail == false)
        {
            Destroy(trail);
        }

        // Called to attach as a subscriber to DataServer.
        server.RegisterDataSubscriber("Drone", this);
    }

    /// <summary>
    /// The callback for receiving data on the subscribed channel.
    /// Parses and checks if message is corrupted.
    /// Stores data ready for visualization.
    /// </summary>
    /// <param name="timestamp">The timestamp of the received message.</param>
    /// <param name="message">The raw contents of the message.</param>
    public void OnReceiveMessage(float timestamp, string message)
    {
        if(string.Compare(message.ToString(),"End of PosData")==0)
        {
            finished = true;
        }
        // Debug.Log("Drone Received: " + message);
        string[] parts = message.Split(',');
        float x, y, z;
        if (parts.Length >= 3 && float.TryParse(parts[0], out x) && 
            float.TryParse(parts[1], out y) && float.TryParse(parts[2], out z))
        {
            newPosition = (flipYZ) ? new Vector3(x, z, y) : new Vector3(x, y, z);
            positionDidUpdate = true;
        }

    }

    /* Update is called once per frame. If we see that we received a new xyz point for the data, we change the position. */
    void Update()
    {
        if (positionDidUpdate)
        {
            positionDidUpdate = false;
            transform.position = newPosition;
        }
        if (finished)
        {
            PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Prefabs/drone_path.prefab");
            finished = false;
        }
    }
}
