using UnityEngine;

public class RoadMaterialGenerator : MonoBehaviour
{
    [Header("Road Appearance")]
    public Color roadColor = new Color(0.15f, 0.15f, 0.15f);
    public Color laneColor = new Color(1f, 1f, 1f);
    public float roadSmoothness = 0.3f;
    public float roadMetallic = 0.1f;
    
    [Header("Road Markings")]
    public float markingWidth = 0.2f;
    public float markingLength = 2f;
    public float markingSpacing = 3f;
    
    void Start()
    {
        GenerateRoadMaterials();
        GenerateLaneMarkings();
    }
    
    void GenerateRoadMaterials()
    {
        // Create road material
        Material roadMaterial = new Material(Shader.Find("Standard"));
        roadMaterial.color = roadColor;
        roadMaterial.SetFloat("_Metallic", roadMetallic);
        roadMaterial.SetFloat("_Glossiness", roadSmoothness);
        
        // Apply to road
        Renderer[] roadRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in roadRenderers)
        {
            if (renderer.gameObject.name.Contains("Road"))
            {
                renderer.material = roadMaterial;
            }
        }
    }
    
    void GenerateLaneMarkings()
    {
        // Create lane marking material
        Material laneMaterial = new Material(Shader.Find("Standard"));
        laneMaterial.color = laneColor;
        
        // Create lane markings along the road
        for (int z = -20; z < 60; z += (int)(markingLength + markingSpacing))
        {
            // Left lane marking
            CreateMarking(-1.5f, z, laneMaterial);
            // Right lane marking
            CreateMarking(1.5f, z, laneMaterial);
        }
    }
    
    void CreateMarking(float x, float z, Material material)
    {
        GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marking.name = "LaneMarking";
        marking.transform.position = new Vector3(x, -0.45f, z);
        marking.transform.localScale = new Vector3(markingWidth, 0.05f, markingLength);
        
        Renderer renderer = marking.GetComponent<Renderer>();
        renderer.material = material;
        
        // Parent to road
        marking.transform.parent = transform;
    }
}