using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite[] sprites;

    public virtual void ChangeSprite(int spriteIndex)
    {
        GetComponentInChildren<SpriteRenderer>().sprite = sprites[spriteIndex];
    }
}
