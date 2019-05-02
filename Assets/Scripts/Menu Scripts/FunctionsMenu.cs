using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionsMenu : MonoBehaviour {

    public GameObject row;

    private Transform windowTransform;
    private float nextOffset = -70;

	// Use this for initialization
	void Start () {
        windowTransform = this.gameObject.transform.GetChild(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addRow(string func, Color color)
    {
        
        //Debug.Log(newPosition);
        //Debug.Log(row.transform.rotation);
        //Debug.Log(windowTransform);

        //GameObject newRow = Instantiate(row, new Vector3(0, 0, 0), Quaternion.identity, windowTransform);
        GameObject newRow = Instantiate(row, windowTransform, false);
        Debug.Log("old pos");
        Debug.Log(newRow.transform.localPosition);
        Debug.Log("new pos");
        newRow.transform.localPosition = new Vector3(newRow.transform.localPosition.x, newRow.transform.localPosition.y + nextOffset, newRow.transform.localPosition.z);
        Debug.Log(newRow.transform.localPosition);
        //GameObject newRow = Instantiate(row, newPosition, row.transform.rotation, windowTransform);
        //newRow.transform.GetChild(0).GetChild(0).GetChild(0).gameObject
        nextOffset -= 70;
    }
}
