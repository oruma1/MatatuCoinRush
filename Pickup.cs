using UnityEngine;

public enum PickupType
{
    Coin,
    NairobiFact
}

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public PickupType type = PickupType.Coin;
    public GameObject collectEffect;
    
    [Header("Visual")]
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    public float rotateSpeed = 90f;

    bool collected;
    float floatTimer;
    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Floating animation
        floatTimer += Time.deltaTime * floatSpeed;
        float yOffset = Mathf.Sin(floatTimer) * floatHeight;
        transform.position = startPosition + new Vector3(0, yOffset, 0);

        // Rotation animation
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        // Pulse scale
        float scale = 1f + Mathf.Sin(floatTimer * 2f) * 0.1f;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void Reset()
    {
        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        if (type == PickupType.Coin)
            GameManager.Instance.CollectCoin(transform.position);
        else
            GameManager.Instance.CollectFactPowerUp();

        if (collectEffect)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}