using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTutorial : MonoBehaviour {

    void Start()
    {
        StartCoroutine(TutorialCoroutine());
    }
    IEnumerator TutorialCoroutine ()
    {

        yield return null;
    }

}
