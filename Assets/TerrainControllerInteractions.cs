using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainControllerInteractions : MonoBehaviour
{

    Terrain terrain;
    Vector3 originalScale;
    Vector3 originalWorldPosition;
    Vector3 originalTerrainPosition;
    Vector3 minScale;
    Vector3 maxScale;
    float speed;

    // DRONE MENU
    private GameObject droneMenu;

    // Use this for initialization
    void Start()
    {
        terrain = Terrain.activeTerrain;

        // DRONE MENU
        droneMenu = GameObject.FindGameObjectWithTag("DroneMenu");
        originalScale = terrain.terrainData.size;
        originalWorldPosition = transform.position;
        originalTerrainPosition = terrain.transform.position;
        minScale = Vector3.Scale(originalScale, new Vector3(0.05F, 0.05F, 0.05F));
        maxScale = Vector3.Scale(originalScale, new Vector3(10F, 10F, 10F));
        speed = 3;

        Vector3 TS = terrain.GetComponent<Terrain>().terrainData.size;
        terrain.transform.position = new Vector3(-TS.x / 2, 1, -TS.z / 2);
        transform.localPosition += originalTerrainPosition - terrain.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            Vector3 d = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) - OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Vector3 v = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch) - OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

            float result = Vector3.Dot(v, d);
            float final_result = 1.0F + 0.2F * result;
            Vector3 terrainScale = Vector3.Scale(terrain.terrainData.size, new Vector3(final_result, final_result, final_result));
            if (terrainScale.sqrMagnitude > minScale.sqrMagnitude && terrainScale.sqrMagnitude < maxScale.sqrMagnitude)
            {
                terrain.terrainData.size = terrainScale;
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(final_result, final_result, final_result));
                // EditorUtility.DisplayDialog("Both Buttons just pressed", "Both Buttons just pressed \n"+result+"\n"+d+"\n"+v, "Okay", "Cancel");
                Vector3 heightAdjustedPos = transform.position;
                //heightAdjustedPos.y = originalTerrainPosition.y + (0.2F * -result);
                heightAdjustedPos.y = originalTerrainPosition.y + (1 - transform.localScale.y);
                transform.position = heightAdjustedPos;
            }
        }
        DeactivateDroneMenu();

        //MOVING SCENE

        float moveX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        float moveZ = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
        //float moveY = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        //float rotate = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;


        // update player position based on input
        Vector3 position = transform.position;
        position.x += moveX * speed * Time.deltaTime;
        //position.y += moveY * speed * Time.deltaTime;
        position.z += moveZ * speed * Time.deltaTime;
        transform.position = position;

        //transform.Rotate(0, rotate*Time.deltaTime*speed*10, 0, Space.Self);

        //Vector3 terrain_position = terrain.transform.position;
        //terrain_position.x += moveHorizontal * speed * Time.deltaTime;
        //terrain_position.z += moveVertical * speed * Time.deltaTime;
        //terrain.transform.position = terrain_position;

    }
    void DeactivateDroneMenu()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
        {
            Debug.Log("Menu Deactivated");
            droneMenu.SetActive(false);
        }
    }
    void OnApplicationQuit()
    {
        terrain.terrainData.size = originalScale;
    }
}
