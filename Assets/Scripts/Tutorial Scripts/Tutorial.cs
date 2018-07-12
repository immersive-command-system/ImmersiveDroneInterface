namespace ISAACS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using VRTK;

    public class Tutorial : MonoBehaviour {

        [Tooltip("The amount of time to wait before tutorial starts")]
        public float seconds;

        public GameObject rightController, leftController;
        public AudioSource introAudio, envAudio, mapLocationAudio, mapRotationAudio, MapScaleAudio, primaryPlacementAudio,
            grabZoneAudio, intermediatePlacementAudio, selectionPointerAudio, secondaryPlacementAudio, undoAndDeleteAudio;
        // Use this for initialization
        void Start() {
            seconds = 4;
            StartCoroutine(TutorialCoroutine());
        }

        IEnumerator TutorialCoroutine()
        {
            yield return new WaitForSecondsRealtime(seconds);
            introAudio.Play();
        }
        // Update is called once per frame
        void Update() {

        }
    }
}
