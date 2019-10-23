using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCanvasResolution : MonoBehaviour
{
    CanvasScaler canvasScaler;

    private void Reset()
    {
        SetReferenceResolution();
    }

    private void OnEnable()
    {
        SetReferenceResolution();
    }

    void SetReferenceResolution()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }
}
