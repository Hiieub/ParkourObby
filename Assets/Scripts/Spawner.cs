using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnDistance = 30f;

    private float[] lanePositions = { -3f, 0f, 3f };

    private float timer;
    private Transform player;

    List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        player = FindAnyObjectByType<Player>().transform;
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            Spawn();
            timer = spawnInterval;
        }

        CleanupBehindPlayer();
    }

    private void Spawn()
    {
        int laneIndex = Random.Range(0, lanePositions.Length);
        float xPos = lanePositions[laneIndex];

        Vector3 spawnPos = new Vector3(xPos, 0.5f, player.position.z + spawnDistance);

        GameObject prefab = Random.value < 0.7f ? obstaclePrefab : coinPrefab;

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        spawnedObjects.Add(obj);
    }

    private void CleanupBehindPlayer()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);
                continue;
            }

            if (spawnedObjects[i].transform.position.z < player.position.z - 10f)
            {
                Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
            }
        }
    }
}
