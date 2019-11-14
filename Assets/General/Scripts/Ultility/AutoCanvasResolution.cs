﻿using UnityEngine;
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

    [ContextMenu("SetResolution")]
    void SetReferenceResolution()
    {
        if (Application.isEditor) return;

        canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }
}
