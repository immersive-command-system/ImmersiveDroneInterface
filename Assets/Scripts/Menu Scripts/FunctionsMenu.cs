using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionsMenu : MonoBehaviour {

    public GameObject row;

    private Transform windowTransform;
    private float nextYPos = 26.6964f;

	// Use this for initialization
	void Start () {
        windowTransform = this.gameObject.transform.GetChild(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addRow(string func, Color color)
    {
        Vector3 newPosition = row.transform.position;
        newPosition.y = nextYPos;
        Debug.Log("heree");
        Debug.Log(newPosition);
        Debug.Log(row.transform.rotation);
        Debug.Log(windowTransform);
        GameObject newRow = Instantiate(row, newPosition, row.transform.rotation, windowTransform);
        nextYPos -= 88;
    }
}
