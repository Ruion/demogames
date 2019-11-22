using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change the sprite image of sprite component randomly
/// Tips: Attach to gameObject with sprite component, call ChangeSpriteRandom()
/// </summary>
public class SpriteChangerRandom : SpriteChanger
{
    public void ChangeSpriteRandom()
    {
        int rand = Random.Range(0, sprites.Length);
        ChangeSprite(rand);
    }
}
