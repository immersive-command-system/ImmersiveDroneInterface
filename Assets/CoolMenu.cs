using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolMenu : MonoBehaviour {

    public GameObject menu;
    public Vector3 originalScale;
    public Vector3 targetScale;
    public Vector3 originalPosition;
    public Vector3 targetPosition;

    void Start()
    {
        menu.SetActive(false);
    }

    IEnumerator LerpUp()
    {
        float progress = 0;
        menu.SetActive(true);

        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            transform.position = Vector3.Lerp(originalPosition, targetPosition, progress);
            progress += Time.deltaTime * 2.0f;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            Debug.Log("YO");
            StartCoroutine("LerpUp");
        }
    }

}
