using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPositioner : MonoBehaviour
{
    public Vector3 position;

    public float xPos;
    public float yPos;
    public float zPos;

    // Start is called before the first frame update
    private void Start()
    {
        position.x = xPos;
        position.y = yPos;
        position.z = zPos;

        RectTransform rectTransform;
        gameObject.TryGetComponent<RectTransform>(out rectTransform);
        if (rectTransform != null)
        {
            rectTransform.localPosition = position;
        }
        else
        {
            transform.position = position;
        }
    }
}