using UnityEngine;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour
{
    [Header("Road Settings")]
    public GameObject roadSegmentPrefab;
    public float segmentLength = 20f;
    public int poolSize = 10;
    
    [Header("Lane Markings")]
    public GameObject laneMarkingPrefab;
    public float markingSpacing = 2f;
    
    [Header("Side Objects")]
    public GameObject[] buildingPrefabs;
    public GameObject[] treePrefabs;
    public GameObject[] streetLampPrefabs;
    public float sideObjectSpacing = 5f;
    
    [Header("Road Decorations")]
    public GameObject potholePrefab;
    public GameObject speedBumpPrefab;
    public float obstacleChance = 0.3f;
    
    private List<GameObject> roadSegments = new List<GameObject>();
    private List<GameObject> sideObjects = new List<GameObject>();
    private float lastSpawnZ = 0f;
    private float destroyZ = -20f;
    private int currentSegmentIndex = 0;
    
    void Start()
    {
        InitializeRoad();
    }
    
    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        
        float speed = GameManager.Instance.CurrentSpeed;
        float moveDistance = speed * Time.deltaTime;
        
        // Move all road objects
        foreach (GameObject segment in roadSegments)
        {
            if (segment != null)
                segment.transform.Translate(Vector3.back * moveDistance, Space.World);
        }
        
        foreach (GameObject obj in sideObjects)
        {
            if (obj != null)
                obj.transform.Translate(Vector3.back * moveDistance, Space.World);
        }
        
        // Recycle segments
        if (roadSegments.Count > 0 && roadSegments[0].transform.position.z < destroyZ)
        {
            RecycleSegment();
        }
    }
    
    void InitializeRoad()
    {
        // Create road segments
        for (int i = 0; i < poolSize; i++)
        {
            float z = i * segmentLength;
            GameObject segment = CreateRoadSegment(z);
            roadSegments.Add(segment);
            
            // Add side objects
            SpawnSideObjects(z);
        }
        
        lastSpawnZ = poolSize * segmentLength;
    }
    
    GameObject CreateRoadSegment(float z)
    {
        if (roadSegmentPrefab == null)
        {
            // Create a basic road if no prefab
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Plane);
            road.transform.position = new Vector3(0, -0.5f, z);
            road.transform.localScale = new Vector3(10f, 1f, segmentLength / 10f);
            
            // Add material
            Renderer renderer = road.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.color = new Color(0.2f, 0.2f, 0.2f);
            
            return road;
        }
        
        GameObject segmentObj = Instantiate(roadSegmentPrefab, new Vector3(0, -0.5f, z), Quaternion.identity);
        return segmentObj;
    }
    
    void SpawnSideObjects(float z)
    {
        // Left side
        for (int side = -1; side <= 1; side += 2)
        {
            float x = side * 6f;
            
            // Random buildings
            if (buildingPrefabs.Length > 0 && Random.value < 0.3f)
            {
                GameObject building = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
                Vector3 pos = new Vector3(x, 0, z + Random.Range(-2f, 2f));
                GameObject obj = Instantiate(building, pos, Quaternion.Euler(0, side > 0 ? 0 : 180, 0));
                sideObjects.Add(obj);
            }
            
            // Trees
            if (treePrefabs.Length > 0 && Random.value < 0.4f)
            {
                GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length)];
                Vector3 pos = new Vector3(x + side * 2f, 0, z + Random.Range(-3f, 3f));
                GameObject obj = Instantiate(tree, pos, Quaternion.identity);
                sideObjects.Add(obj);
            }
            
            // Street lamps
            if (streetLampPrefabs.Length > 0 && Random.value < 0.2f)
            {
                GameObject lamp = streetLampPrefabs[Random.Range(0, streetLampPrefabs.Length)];
                Vector3 pos = new Vector3(x + side * 1f, 0, z + Random.Range(-1f, 1f));
                GameObject obj = Instantiate(lamp, pos, Quaternion.identity);
                sideObjects.Add(obj);
            }
        }
        
        // Potholes on road
        if (potholePrefab != null && Random.value < obstacleChance)
        {
            int lane = Random.Range(-1, 2);
            Vector3 pos = new Vector3(lane * 3f, 0.05f, z + Random.Range(-1f, 1f));
            GameObject pothole = Instantiate(potholePrefab, pos, Quaternion.identity);
            sideObjects.Add(pothole);
        }
    }
    
    void RecycleSegment()
    {
        if (roadSegments.Count == 0) return;
        
        // Get oldest segment
        GameObject oldSegment = roadSegments[0];
        roadSegments.RemoveAt(0);
        
        // Reposition to end
        float newZ = lastSpawnZ;
        oldSegment.transform.position = new Vector3(0, -0.5f, newZ);
        
        // Reset any child objects
        foreach (Transform child in oldSegment.transform)
        {
            if (child.tag == "Obstacle")
            {
                child.gameObject.SetActive(true);
            }
        }
        
        roadSegments.Add(oldSegment);
        
        // Spawn new side objects at end
        SpawnSideObjects(newZ);
        
        lastSpawnZ += segmentLength;
    }
    
    void OnDestroy()
    {
        // Clean up
        foreach (GameObject obj in roadSegments)
        {
            if (obj != null) Destroy(obj);
        }
        
        foreach (GameObject obj in sideObjects)
        {
            if (obj != null) Destroy(obj);
        }
    }
}