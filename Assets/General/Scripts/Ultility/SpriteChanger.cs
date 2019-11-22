using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change sprite of a sprite component to the selected sprite inside sprites array.
/// Tips: Attach to gameObject with sprite component, drag sprite images into "sprites", call ChangeSprite(desireIndex)
/// </summary>
public class SpriteChanger : MonoBehaviour
{
    public Sprite[] sprites;

    public virtual void ChangeSprite(int spriteIndex)
    {
        GetComponentInChildren<SpriteRenderer>().sprite = sprites[spriteIndex];
    }
}
