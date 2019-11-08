using UnityEngine;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;

public class RandomProbabilitySpawner : MonoBehaviour
{
    public SpriteChangerRandom prefab;
    public bool spawnOnEnable = true;
    public float verticalPosBuffer = .5f;

    [Range(0f, 1f)]
    public float spawnProbability = .5f;

    public void OnEnable()
    {
       if(spawnOnEnable) Spawn();
    }

    public void Spawn()
    {
        float horizontalPos = Random.Range(-3.2f, 3.2f);
        Vector2 position = new Vector2(transform.position.x + horizontalPos, transform.position.y + verticalPosBuffer);
        //  Vector2 position = new Vector2(transform.position.x , transform.position.y );

        //  if (Random.value <= spawnProbability) Instantiate(prefab, position, prefab.transform.rotation);
        // if (Random.value <= spawnProbability) { Instantiate(prefab, transform); StartCoroutine(ChangeGroundType()); }
        if (Random.value <= spawnProbability) { Instantiate(prefab, position, prefab.transform.rotation); }
    }

    private IEnumerator ChangeGroundType()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<GroundScript>().groundType = GroundScript.GroundType.Shoe;
    }
}
