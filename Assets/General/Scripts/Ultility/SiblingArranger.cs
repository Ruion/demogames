using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiblingArranger : MonoBehaviour
{
    public int siblingIndex;

    
    void Start()
    {
        SetSiblingIndex();
    }

    [ContextMenu("SetSibling")]
    public void SetSiblingIndex()
    {
        transform.SetSiblingIndex(siblingIndex);
    }
}
