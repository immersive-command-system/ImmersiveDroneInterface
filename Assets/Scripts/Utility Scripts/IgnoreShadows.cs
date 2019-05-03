using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreShadows : MonoBehaviour {

    float shadowDist;

    /// <summary>
    /// PreRender method for minimap camera. Stores the "shadowDistance" variable from QualitySettings for later and sets it to 0 for now
    /// This prevents the minimap camera from rendering any shadows.
    /// </summary>
    void OnPreRender(){
        shadowDist = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }

    /// <summary>
    /// PostRender method for minimap camera. Restores the "shadowDistance" variable to its old value so that all other shadows after this render correctly.
    /// </summary>
    void OnPostRender(){
        QualitySettings.shadowDistance = shadowDist;
    }
}
