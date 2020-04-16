using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverChangeSprite : MonoBehaviour
{
    public Sprite newSprite;

    private void OnMouseOver()
    {
        transform.GetComponent<SpriteRenderer>().sprite = newSprite;
        Debug.Log(name + " changing sprite");
    }
}