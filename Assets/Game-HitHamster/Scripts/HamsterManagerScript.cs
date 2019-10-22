using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterManagerScript : MonoBehaviour {
	// Spawn frequency will increase when the timeremaining nearer to 0.
	// Spawnvariacne is to adjust the spawnfrequency, so it became random while moving towards became faster
	[Header("Hamster Spawn Setting")]
	[Tooltip("Max number of instance spawn at the same time")]
	public int maxInstance;
	[Tooltip("Spawn frequency will increase when the timeremaining nearer to 0. Spawnvariacne is to adjust the spawnfrequency, so it became random while moving towards became faster")]
	public float spawnVariance;
	[Tooltip("List of available spawn object")]
	public List<GameObject> objectPrefabs;
	[Tooltip("List of available spawn point")]
	public List<GameObject> spawnPoints;

	[Header("Debug Purpose")]
	[ReadOnly] public int currentInstance;
	[ReadOnly] public float instanceFrequency;

	private GameObject spawnedObject;
	private GameObject spawnPoint;
	private Vector3 position;
	private HamsterGameManagerScript gameManager;
	void Awake(){
		currentInstance = 0;
		gameManager = GameObject.Find("GameManager").GetComponent<HamsterGameManagerScript>();
	}

    public void StartGame()
    {
        StartCoroutine(GeneratePrefab());
        StartCoroutine(GeneratePrefab());
       // GeneratePrefabCoroutine();
    }

	void GeneratePrefabCoroutine(){
        StartCoroutine(GeneratePrefab());
    }

	public void RemovedInstance(){
		currentInstance -= 1;
		gameManager.AddDifficulty();
	}

	IEnumerator GeneratePrefab(){
        while (gameManager.loseAlready() == false)
        {
            instanceFrequency = gameManager.getMaxLevel() - gameManager.getCurrentLevel();

            float second = Mathf.Max(0, instanceFrequency + Random.Range(-spawnVariance, spawnVariance));

            yield return new WaitForSeconds(second);

            // If still allow to spwn instance
            if (currentInstance < maxInstance)
            {
                spawnedObject = objectPrefabs[Random.Range(0, objectPrefabs.Count)];

                do
                {
                    spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                } while (spawnPoint.transform.childCount > 2); // number "2" has to been same as child number of SpawnPoint

                currentInstance += 1;
                position = spawnPoint.transform.position;
                GameObject newObject = Instantiate(spawnedObject, position, Quaternion.identity);
                newObject.transform.SetParent(spawnPoint.transform);
            }
        }
	}
}
