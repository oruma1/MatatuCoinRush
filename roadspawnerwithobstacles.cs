using UnityEngine;
using System.Collections.Generic;

public class RoadSpawner : MonoBehaviour
{
    [Header("Lane Setup")]
    public float laneWidth = 3f;
    public float spawnZ = 45f;
    public float destroyZ = -12f;
    
    [Header("Prefabs")]
    public GameObject roadPrefab;
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public GameObject coinPrefab;
    public GameObject factPowerUpPrefab;
    
    [Header("Road Decorations")]
    public GameObject[] buildingPrefabs;
    public GameObject[] treePrefabs;
    public GameObject[] streetFurniture;
    
    [Header("Spawn Settings")]
    public float spawnInterval = 0.65f;
    public float coinSpacing = 2.8f;
    public int minCoinsInLine = 3;
    public int maxCoinsInLine = 5;
    [Range(0f, 1f)] public float factChance = 0.18f;
    [Range(0f, 1f)] public float obstacleDensity = 0.4f;
    
    private float spawnTimer;
    private List<GameObject> activeObjects = new List<GameObject>();
    private List<GameObject> environmentObjects = new List<GameObject>();
    
    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnPattern();
            float speedFactor = Mathf.InverseLerp(
                GameManager.Instance.baseSpeed,
                GameManager.Instance.maxSpeed,
                GameManager.Instance.CurrentSpeed
            );
            spawnTimer = Mathf.Lerp(spawnInterval, spawnInterval * 0.5f, speedFactor);
        }
        
        // Cleanup objects that passed behind
        CleanupObjects();
    }
    
    void SpawnPattern()
    {
        List<int> lanes = new List<int> { -1, 0, 1 };
        Shuffle(lanes);
        
        // Determine obstacle count based on difficulty
        int obstacleCount = Random.value < obstacleDensity ? 2 : 1;
        
        // Spawn obstacles
        for (int i = 0; i < obstacleCount && i < lanes.Count; i++)
        {
            SpawnObstacle(lanes[i], spawnZ + i * 2f);
        }
        
        // Spawn coins on a safe lane
        int coinLane = lanes[obstacleCount < lanes.Count ? obstacleCount : Random.Range(0, lanes.Count)];
        int coinCount = Random.Range(minCoinsInLine, maxCoinsInLine + 1);
        for (int i = 0; i < coinCount; i++)
        {
            SpawnPickup(coinPrefab, coinLane, spawnZ + 5f + i * coinSpacing);
        }
        
        // Spawn fact power-up
        if (factPowerUpPrefab != null && Random.value < factChance)
        {
            int factLane = lanes[Random.Range(0, lanes.Count)];
            SpawnPickup(factPowerUpPrefab, factLane, spawnZ + 10f);
        }
        
        // Spawn environment
        SpawnEnvironment();
    }
    
    void SpawnObstacle(int lane, float z)
    {
        if (obstaclePrefabs.Count == 0) return;
        
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
        Vector3 position = new Vector3(lane * laneWidth, 0.5f, z);
        GameObject obstacle = Instantiate(prefab, position, Quaternion.identity);
        
        // Random rotation for variety
        obstacle.transform.Rotate(0, Random.Range(-30f, 30f), 0);
        
        // Add RunnerObject if not present
        if (!obstacle.TryGetComponent(out RunnerObject runner))
        {
            runner = obstacle.AddComponent<RunnerObject>();
            runner.destroyZ = destroyZ;
        }
        
        activeObjects.Add(obstacle);
    }
    
    void SpawnPickup(GameObject prefab, int lane, float z)
    {
        if (prefab == null) return;
        
        Vector3 position = new Vector3(lane * laneWidth, 1f, z);
        GameObject pickup = Instantiate(prefab, position, Quaternion.identity);
        
        // Add RunnerObject if not present
        if (!pickup.TryGetComponent(out RunnerObject runner))
        {
            runner = pickup.AddComponent<RunnerObject>();
            runner.destroyZ = destroyZ;
        }
        
        activeObjects.Add(pickup);
    }
    
    void SpawnEnvironment()
    {
        // Spawn buildings on both sides
        for (int side = -1; side <= 1; side += 2)
        {
            if (buildingPrefabs.Length > 0 && Random.value < 0.3f)
            {
                GameObject building = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                float x = side * (laneWidth * 2f + Random.Range(2f, 5f));
                Vector3 pos = new Vector3(x, 0, spawnZ + Random.Range(-5f, 5f));
                GameObject envObj = Instantiate(building, pos, Quaternion.Euler(0, side > 0 ? 180 : 0, 0));
                environmentObjects.Add(envObj);
            }
        }
        
        // Spawn trees
        if (treePrefabs.Length > 0 && Random.value < 0.4f)
        {
            float x = Random.Range(-laneWidth * 3f, laneWidth * 3f);
            if (Mathf.Abs(x) > laneWidth * 1.5f) // Avoid road
            {
                GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length)];
                Vector3 pos = new Vector3(x, 0, spawnZ + Random.Range(-3f, 3f));
                GameObject envObj = Instantiate(tree, pos, Quaternion.identity);
                environmentObjects.Add(envObj);
            }
        }
        
        // Spawn road
        if (roadPrefab != null)
        {
            Vector3 pos = new Vector3(0, -0.5f, spawnZ);
            GameObject road = Instantiate(roadPrefab, pos, Quaternion.identity);
            environmentObjects.Add(road);
        }
    }
    
    void CleanupObjects()
    {
        // Cleanup active objects
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null || activeObjects[i].transform.position.z < destroyZ)
            {
                if (activeObjects[i] != null)
                    Destroy(activeObjects[i]);
                activeObjects.RemoveAt(i);
            }
        }
        
        // Cleanup environment objects
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
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}