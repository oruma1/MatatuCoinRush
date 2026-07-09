using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour
{
    public GameObject hitEffect;
    public GameObject obstacleMesh;
    public AudioClip impactSound;

    [Header("Animation")]
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 0.1f;

    Vector3 originalPosition;
    bool isShaking = false;
    float shakeTimer = 0f;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                isShaking = false;
                transform.position = originalPosition;
            }
            else
            {
                transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
            }
        }
    }

    void Reset()
    {
        Collider obstacleCollider = GetComponent<Collider>();
        obstacleCollider.isTrigger = true;
    }

    public void Hit()
    {
        // Visual shake effect
        isShaking = true;
        shakeTimer = shakeDuration;

        // Particle effect
        if (hitEffect)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Hide obstacle
        if (obstacleMesh != null)
            obstacleMesh.SetActive(false);

        GameManager.Instance.HitObstacle();
        
        // Destroy after delay
        Destroy(gameObject, 1f);
    }
}