using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MatatuBusPlayer : MonoBehaviour
{
    [Header("Lane Movement")]
    public float laneWidth = 3f;
    public float laneChangeSpeed = 12f;
    public float leanAngle = 12f;
    
    [Header("Visual Effects")]
    public GameObject busModel;
    public Light headlightLeft;
    public Light headlightRight;
    public ParticleSystem exhaustParticles;
    public ParticleSystem tireSparks;
    
    [Header("Bus Animation")]
    public float wheelRotationSpeed = 100f;
    public Transform[] wheels; // Front and rear wheels
    public float suspensionBounce = 0.05f;
    
    [Header("Touch Controls")]
    public float swipeThreshold = 50f;
    
    [Header("Audio")]
    public AudioSource engineAudio;
    public AudioSource hornAudio;
    public AudioClip hornSound;
    public AudioClip boostSound;
    
    int targetLane;
    Vector2 touchStart;
    bool touching;
    float wheelRotation = 0f;
    float bounceTimer = 0f;
    float currentSpeed = 0f;
    bool isBoosting = false;
    Vector3 originalBusPosition;
    
    void Start()
    {
        if (busModel == null)
            busModel = gameObject;
            
        originalBusPosition = busModel.transform.localPosition;
        
        // Start engine sound
        if (engineAudio != null)
            engineAudio.Play();
    }
    
    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;
        
        currentSpeed = GameManager.Instance.CurrentSpeed;
        isBoosting = GameManager.Instance.FactBoostRemaining > 0f;
        
        ReadKeyboard();
        ReadTouch();
        MoveToLane();
        AnimateBus();
        UpdateHeadlights();
        UpdateExhaust();
        UpdateEngineSound();
    }
    
    void ReadKeyboard()
    {
        // Lane changes
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);
            
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ChangeLane(1);
            
        // Horn
        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Space))
            HonkHorn();
    }
    
    void ReadTouch()
    {
        if (Input.touchCount <= 0) return;
        
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            touchStart = touch.position;
            touching = true;
        }
        else if (touching && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            Vector2 delta = touch.position - touchStart;
            if (Mathf.Abs(delta.x) > swipeThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                ChangeLane(delta.x > 0 ? 1 : -1);
            else if (Mathf.Abs(delta.y) > swipeThreshold && Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                HonkHorn();
                
            touching = false;
        }
    }
    
    public void ChangeLane(int direction)
    {
        targetLane = Mathf.Clamp(targetLane + direction, -1, 1);
        
        // Tire sparks when changing lanes
        if (tireSparks != null)
            tireSparks.Play();
    }
    
    void MoveToLane()
    {
        Vector3 target = transform.position;
        target.x = targetLane * laneWidth;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * laneChangeSpeed);
        
        // Bus lean
        float lean = Mathf.Clamp((target.x - transform.position.x) / laneWidth, -1f, 1f) * -leanAngle;
        if (busModel != null)
        {
            busModel.transform.rotation = Quaternion.Lerp(
                busModel.transform.rotation,
                Quaternion.Euler(0f, 0f, lean),
                Time.deltaTime * 10f
            );
        }
    }
    
    void AnimateBus()
    {
        // Wheel rotation
        wheelRotation += Time.deltaTime * wheelRotationSpeed * (currentSpeed / 10f);
        
        foreach (Transform wheel in wheels)
        {
            if (wheel != null)
            {
                wheel.localRotation = Quaternion.Euler(wheelRotation, 0f, 0f);
            }
        }
        
        // Suspension bounce
        bounceTimer += Time.deltaTime * (currentSpeed / 5f);
        float bounce = Mathf.Sin(bounceTimer) * suspensionBounce;
        busModel.transform.localPosition = originalBusPosition + new Vector3(0, bounce, 0);
    }
    
    void UpdateHeadlights()
    {
        bool lightsOn = currentSpeed > 5f || isBoosting;
        
        if (headlightLeft != null)
            headlightLeft.enabled = lightsOn;
            
        if (headlightRight != null)
            headlightRight.enabled = lightsOn;
    }
    
    void UpdateExhaust()
    {
        if (exhaustParticles != null)
        {
            if (isBoosting)
            {
                var emission = exhaustParticles.emission;
                emission.rateOverTime = 50f;
                
                // Make exhaust glow during boost
                var main = exhaustParticles.main;
                main.startColor = new Color(1f, 0.8f, 0.2f, 0.5f);
            }
            else
            {
                var emission = exhaustParticles.emission;
                emission.rateOverTime = 15f;
                
                var main = exhaustParticles.main;
                main.startColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            }
        }
    }
    
    void UpdateEngineSound()
    {
        if (engineAudio != null)
        {
            float pitch = Mathf.Lerp(0.5f, 1.5f, currentSpeed / GameManager.Instance.maxSpeed);
            engineAudio.pitch = pitch;
            
            float volume = Mathf.Lerp(0.3f, 1f, currentSpeed / GameManager.Instance.maxSpeed);
            engineAudio.volume = volume;
        }
    }
    
    void HonkHorn()
    {
        if (hornAudio != null && hornSound != null)
        {
            hornAudio.PlayOneShot(hornSound, 0.7f);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pickup pickup))
        {
            pickup.Collect();
            return;
        }
        
        if (other.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.Hit();
        }
    }
}