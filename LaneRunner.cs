using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaneRunnerPlayer : MonoBehaviour
{
    [Header("Lane Movement")]
    public float laneWidth = 3f;
    public float laneChangeSpeed = 12f;
    public float leanAngle = 12f;

    [Header("Touch")]
    public float swipeThreshold = 50f;

    [Header("Animation")]
    public Animator playerAnimator;
    public GameObject playerModel;
    public float jumpHeight = 1.5f;
    public float jumpDuration = 0.5f;

    [Header("Particles")]
    public ParticleSystem trailParticles;
    public ParticleSystem jumpParticles;

    int targetLane;
    Vector2 touchStart;
    bool touching;
    bool isJumping = false;
    float jumpTimer = 0f;
    Vector3 jumpStartPosition;

    void Start()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsRunning) return;

        ReadKeyboard();
        ReadTouch();
        MoveToLane();
        HandleJump();
        UpdateAnimation();

        // Trail particles
        if (trailParticles != null)
        {
            if (GameManager.Instance.CurrentSpeed > GameManager.Instance.baseSpeed)
            {
                if (!trailParticles.isPlaying)
                    trailParticles.Play();
            }
            else
            {
                if (trailParticles.isPlaying)
                    trailParticles.Stop();
            }
        }
    }

    void ReadKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            ChangeLane(1);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Jump();
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
                Jump();

            touching = false;
        }
    }

    public void ChangeLane(int direction)
    {
        targetLane = Mathf.Clamp(targetLane + direction, -1, 1);
    }

    public void Jump()
    {
        if (isJumping) return;
        isJumping = true;
        jumpTimer = 0f;
        jumpStartPosition = transform.position;

        // Jump particles
        if (jumpParticles != null)
        {
            jumpParticles.transform.position = transform.position;
            jumpParticles.Play();
        }

        // Animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Jump");
    }

    void HandleJump()
    {
        if (!isJumping) return;

        jumpTimer += Time.deltaTime;
        float progress = jumpTimer / jumpDuration;

        if (progress >= 1f)
        {
            isJumping = false;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            return;
        }

        float height = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    void MoveToLane()
    {
        Vector3 target = transform.position;
        target.x = targetLane * laneWidth;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * laneChangeSpeed);

        float lean = Mathf.Clamp((target.x - transform.position.x) / laneWidth, -1f, 1f) * -leanAngle;
        if (playerModel != null)
        {
            playerModel.transform.rotation = Quaternion.Lerp(
                playerModel.transform.rotation, 
                Quaternion.Euler(0f, 0f, lean), 
                Time.deltaTime * 10f
            );
        }
        else
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation, 
                Quaternion.Euler(0f, 0f, lean), 
                Time.deltaTime * 10f
            );
        }
    }

    void UpdateAnimation()
    {
        if (playerAnimator == null) return;

        float speed = Mathf.Clamp(GameManager.Instance.CurrentSpeed / GameManager.Instance.maxSpeed, 0, 1);
        playerAnimator.SetFloat("Speed", speed);
        playerAnimator.SetBool("IsRunning", GameManager.Instance.IsRunning);
        playerAnimator.SetBool("IsBoosting", GameManager.Instance.FactBoostRemaining > 0f);
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