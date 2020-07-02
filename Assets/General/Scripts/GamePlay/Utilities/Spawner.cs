using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace General
{
    public class Spawner : SerializedMonoBehaviour
    {
        public Transform[] spawnPoint;
        public Transform spawnParent;
        public GameObject[] entityPrefab;

        private int spawnNumber;
        private int spawnPointNumber;
        private int lastSpawnPointNumber;

        public float minSpawnInterval = .4f;

        public float maxSpawnInterval = .8f;

        public float minSpeed = 2f;
        public float maxSpeed = 5f;

        public class EntityPrefab
        {
            [HorizontalGroup]
            public GameObject entityPrefab;

            [HorizontalGroup]
            public float spawnPercentage;
        }

        public EntityPrefab[] entityPrefabs;

        private List<EntityPrefab> entityPrefabsList;

        public void StartGame()
        {
            Initialize();
            StartCoroutine(StartSpawn());
        }

        [Button]
        private void Initialize()
        {
            entityPrefabsList = new List<EntityPrefab>();

            for (int y = 0; y < entityPrefabs.Length; y++)
            {
                EntityPrefab ep = new EntityPrefab() { entityPrefab = entityPrefabs[y].entityPrefab, spawnPercentage = entityPrefabs[y].spawnPercentage };
                entityPrefabsList.Add(ep);
            }

            for (int e = 1; e < entityPrefabsList.Count; e++)
            {
                entityPrefabsList[e].spawnPercentage += entityPrefabsList[e - 1].spawnPercentage;
            }
        }

        private IEnumerator StartSpawn()
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            // Spawn new entity on another lane
            int newLane = GenerateRandomNumber(spawnPointNumber, spawnPoint);
            while (newLane == lastSpawnPointNumber)
            {
                newLane = GenerateRandomNumber(spawnPointNumber, spawnPoint);
            }
            lastSpawnPointNumber = spawnPointNumber = newLane;

            GameObject newEntity;

            // spawn prefab

            float spawnPercentage = Random.Range(0f, 1f);

            // select first item which percentage >= spawnPercentage from descending order
            spawnNumber = entityPrefabsList.IndexOf(entityPrefabsList.OrderByDescending(e => e.spawnPercentage >= spawnPercentage).FirstOrDefault());

            newEntity = Instantiate(entityPrefabsList[spawnNumber].entityPrefab, spawnPoint[spawnPointNumber].position, Quaternion.identity, spawnParent);

            newEntity.GetComponentInChildren<ObjectMover>().speed = Random.Range(minSpeed, maxSpeed);

            StartCoroutine(StartSpawn());
        }

        private int GenerateRandomNumber(int spawnNumber_, Object[] objects)
        {
            int rand = Random.Range(0, objects.Length);

            return rand;
        }
    }
}