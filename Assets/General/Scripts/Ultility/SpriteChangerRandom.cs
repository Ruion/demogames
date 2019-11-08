using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChangerRandom : SpriteChanger
{
    public void ChangeSpriteRandom()
    {
        int rand = Random.Range(0, sprites.Length);
        ChangeSprite(rand);
    }
}
