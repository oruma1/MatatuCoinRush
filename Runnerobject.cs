using UnityEngine;

public class RunnerObject : MonoBehaviour
{
    public float destroyZ = -12f;
    public bool spin;
    public Vector3 spinSpeed = new(0f, 180f, 0f);
    
    [Header("Visual")]
    public bool bob = false;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    float bobTimer;
    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;

        float speed = GameManager.Instance.CurrentSpeed;
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (spin)
            transform.Rotate(spinSpeed * Time.deltaTime, Space.Self);

        if (bob)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float yOffset = Mathf.Sin(bobTimer) * bobHeight;
            transform.position = startPosition + new Vector3(0, yOffset, 0);
        }

        if (transform.position.z < destroyZ)
            Destroy(gameObject);
    }
}