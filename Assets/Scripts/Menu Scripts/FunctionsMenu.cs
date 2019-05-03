namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine;

    public class FunctionsMenu : MonoBehaviour
    {

        public GameObject row;



        private Transform windowTransform;
        private float nextOffset = 0;

        // Use this for initialization
        void Awake()
        {
            windowTransform = transform.GetChild(0);
        }

        /// <summary>
        /// This method is called in World Properties for each functionality added. Creates a row in the Functions Menu with corresponding 
        /// function name and associated color.
        /// </summary>
        /// <param name="func"> The name of the functionality in this row. </param>
        /// <param name="color"> The color associated with this funcitonality. </param>
        public void addRow(string func, Color color)
        {
            //GameObject newRow = Instantiate(row, new Vector3(0, 0, 0), Quaternion.identity, windowTransform);
            //GameObject newRow = Instantiate(row, transform, true);
            GameObject newRow = Instantiate(row, windowTransform, false);
            print(windowTransform);

            //Debug.Log("hello");
            //Debug.Log(newRow.transform.localPosition);
            //Debug.Log("new pos");
            newRow.transform.localPosition = new Vector3(newRow.transform.localPosition.x, newRow.transform.localPosition.y + nextOffset, newRow.transform.localPosition.z);
            //Debug.Log(newRow.transform.localPosition);

            //Debug.Log(newRow.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);

            GameObject funcLabel = newRow.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            funcLabel.GetComponent<Text>().text = func;
            GameObject colorLabel = newRow.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;
            //colorLabel.GetComponent<Text>().text = color.ToString();
            colorLabel.GetComponent<Text>().text = "O";
            colorLabel.GetComponent<Text>().color = color;

            nextOffset -= 70;
        }
    }
}

