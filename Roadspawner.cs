using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    [Header("Lane Setup")]
    public float laneWidth = 3f;
    public float spawnZ = 45f;
    public float destroyZ = -12f;

    [Header("Prefabs")]
    public List<GameObject> obstaclePrefabs = new();
    public GameObject coinPrefab;
    public GameObject factPowerUpPrefab;

    [Header("Environment")]
    public GameObject[] buildingPrefabs;
    public GameObject[] treePrefabs;
    public GameObject roadSegmentPrefab;
    public float environmentSpawnInterval = 10f;

    [Header("Spawn Rhythm")]
    public float spawnEvery = 0.65f;
    public float coinSpacing = 2.8f;
    public int minCoinsInLine = 3;
    public int maxCoinsInLine = 5;
    [Range(0f, 1f)] public float factChance = 0.18f;

    float spawnTimer;
    float environmentTimer;
    float lastEnvironmentZ = 0f;
    List<GameObject> environmentObjects = new();

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;

        // Spawn obstacles and pickups
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnPattern();
            float speedFactor = Mathf.InverseLerp(GameManager.Instance.baseSpeed, GameManager.Instance.maxSpeed, GameManager.Instance.CurrentSpeed);
            spawnTimer = Mathf.Lerp(spawnEvery, spawnEvery * 0.62f, speedFactor);
        }

        // Spawn environment
        environmentTimer -= Time.deltaTime;
        if (environmentTimer <= 0f)
        {
            SpawnEnvironment();
            environmentTimer = environmentSpawnInterval;
        }

        CleanupEnvironment();
    }

    void SpawnPattern()
    {
        List<int> lanes = new() { -1, 0, 1 };
        Shuffle(lanes);

        int blockedCount = Random.value < 0.35f ? 2 : 1;
        for (int i = 0; i < blockedCount; i++)
            SpawnObstacle(lanes[i], spawnZ + i * 2f);

        int coinLane = lanes[blockedCount < lanes.Count ? blockedCount : Random.Range(0, lanes.Count)];
        int coinCount = Random.Range(minCoinsInLine, maxCoinsInLine + 1);
        for (int i = 0; i < coinCount; i++)
            Spawn(coinPrefab, coinLane, spawnZ + 5f + i * coinSpacing);

        if (factPowerUpPrefab && Random.value < factChance)
        {
            int factLane = lanes[Random.Range(0, lanes.Count)];
            Spawn(factPowerUpPrefab, factLane, spawnZ + 10f);
        }
    }

    void SpawnObstacle(int lane, float z)
    {
        if (obstaclePrefabs.Count == 0) return;
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
        Spawn(prefab, lane, z);
    }

    void SpawnEnvironment()
    {
        // Spawn buildings on sides
        for (int side = -1; side <= 1; side += 2)
        {
            if (buildingPrefabs.Length > 0)
            {
                GameObject building = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                Vector3 position = new Vector3(side * (laneWidth * 2f + Random.Range(3f, 8f)), 0, spawnZ + Random.Range(-5f, 5f));
                GameObject envObj = Instantiate(building, position, Quaternion.Euler(0, side > 0 ? 180 : 0, 0));
                environmentObjects.Add(envObj);
            }
        }

        // Spawn trees randomly
        if (treePrefabs.Length > 0 && Random.value < 0.5f)
        {
            GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Vector3 position = new Vector3(Random.Range(-laneWidth * 3f, laneWidth * 3f), 0, spawnZ + Random.Range(-3f, 3f));
            if (Mathf.Abs(position.x) > laneWidth * 1.2f) // Don't spawn on road
            {
                GameObject envObj = Instantiate(tree, position, Quaternion.identity);
                environmentObjects.Add(envObj);
            }
        }

        // Spawn road segment
        if (roadSegmentPrefab != null)
        {
            Vector3 position = new Vector3(0, -0.5f, spawnZ);
            GameObject road = Instantiate(roadSegmentPrefab, position, Quaternion.identity);
            environmentObjects.Add(road);
        }
    }

    void Spawn(GameObject prefab, int lane, float z)
    {
        if (!prefab) return;
        Vector3 position = new(lane * laneWidth, prefab.transform.position.y, z);
        GameObject instance = Instantiate(prefab, position, prefab.transform.rotation);

        if (!instance.TryGetComponent(out RunnerObject runnerObject))
            runnerObject = instance.AddComponent<RunnerObject>();

        runnerObject.destroyZ = destroyZ;
    }

    void CleanupEnvironment()
    {
        for (int i = environmentObjects.Count - 1; i >= 0; i--)
        {
            if (environmentObjects[i] == null || environmentObjects[i].transform.position.z < destroyZ - 10f)
            {
                if (environmentObjects[i] != null)
                    Destroy(environmentObjects[i]);
                environmentObjects.RemoveAt(i);
            }
        }
    }

    static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}