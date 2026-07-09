using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Run Balance")]
    public float baseSpeed = 8f;
    public float maxSpeed = 18f;
    public float difficultyRampPerSecond = 0.035f;
    public int coinValue = 10;
    public float factBoostDuration = 3.2f;

    [Header("UI")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TMP_Text scoreText;
    public TMP_Text coinText;
    public TMP_Text bestText;
    public TMP_Text factText;
    public TMP_Text boostText;
    public TMP_Text gameOverText;

    [Header("Nairobi Facts")]
    [TextArea] public List<string> nairobiFacts = new()
    {
        "Nairobi means 'cool water' from the Maasai phrase Enkare Nairobi.",
        "Nairobi began as a railway depot in 1899 before becoming Kenya's capital.",
        "Nairobi National Park sits just outside the city centre.",
        "KICC is one of Nairobi's most recognizable skyline landmarks.",
        "Matatus are famous for bold art, music, and Nairobi street style.",
        "Karura Forest is one of Nairobi's major urban forests.",
        "Nairobi is often called the Green City in the Sun.",
        "The Giraffe Centre helps protect endangered Rothschild's giraffes."
    };

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;
    public AudioClip collectSound;
    public AudioClip factSound;
    public AudioClip obstacleHitSound;
    public AudioClip boostSound;

    [Header("Particles")]
    public ParticleSystem coinParticles;
    public ParticleSystem factParticles;
    public ParticleSystem boostParticles;
    public ParticleSystem obstacleParticles;

    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float FactBoostRemaining { get; private set; }

    int score;
    int coins;
    int best;
    int streak;
    float runTime;
    float factMessageTimer;
    bool isBoostActive = false;

    const string BestKey = "MatatuCoinRushBest";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        best = PlayerPrefs.GetInt(BestKey, 0);
        CurrentSpeed = baseSpeed;
        Time.timeScale = 0f;
        ShowStart();
        UpdateUI();
        
        // Play menu music
        if (musicSource != null && gameMusic != null)
        {
            musicSource.clip = gameMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (!IsRunning) return;

        runTime += Time.deltaTime;
        CurrentSpeed = Mathf.Min(maxSpeed, baseSpeed + runTime * difficultyRampPerSecond * baseSpeed);
        score += Mathf.RoundToInt((CurrentSpeed * Time.deltaTime) * (1 + streak / 10));

        if (FactBoostRemaining > 0f)
        {
            FactBoostRemaining -= Time.deltaTime;
            if (!isBoostActive && boostParticles != null)
            {
                boostParticles.Play();
                isBoostActive = true;
            }
        }
        else
        {
            if (isBoostActive && boostParticles != null)
            {
                boostParticles.Stop();
                isBoostActive = false;
            }
        }

        if (factMessageTimer > 0f)
            factMessageTimer -= Time.deltaTime;

        UpdateUI();
    }

    public void StartRun()
    {
        score = 0;
        coins = 0;
        streak = 0;
        runTime = 0f;
        FactBoostRemaining = 0f;
        IsRunning = true;
        IsGameOver = false;
        Time.timeScale = 1f;

        if (startPanel) startPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (factText) factText.gameObject.SetActive(false);
        
        // Play game music
        if (musicSource != null && gameMusic != null)
        {
            musicSource.clip = gameMusic;
            musicSource.Play();
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CollectCoin(Vector3 worldPosition)
    {
        streak++;
        coins++;
        int multiplier = Mathf.Clamp(1 + streak / 8, 1, 5);
        int boostMultiplier = FactBoostRemaining > 0f ? 2 : 1;
        int points = coinValue * multiplier * boostMultiplier;
        score += points;

        // Audio feedback
        PlaySound(collectSound, 0.5f);
        
        // Particle effect
        if (coinParticles != null)
        {
            coinParticles.transform.position = worldPosition;
            coinParticles.Play();
        }

        UpdateUI();
    }

    public void CollectFactPowerUp()
    {
        FactBoostRemaining = factBoostDuration;
        score += 50;

        // Audio feedback
        PlaySound(factSound, 0.7f);
        PlaySound(boostSound, 0.5f);

        // Particle effect
        if (factParticles != null)
        {
            factParticles.Play();
        }

        if (nairobiFacts.Count > 0 && factText)
        {
            factText.text = nairobiFacts[Random.Range(0, nairobiFacts.Count)];
            factText.gameObject.SetActive(true);
            factMessageTimer = 4.5f;
        }

        UpdateUI();
    }

    public void HitObstacle()
    {
        if (!IsRunning || IsGameOver) return;

        if (FactBoostRemaining > 0f)
        {
            score += 35;
            PlaySound(boostSound, 0.3f);
            return;
        }

        // Audio feedback
        PlaySound(obstacleHitSound, 0.8f);
        PlaySound(gameOverMusic, 0.6f);

        // Particle effect
        if (obstacleParticles != null)
        {
            obstacleParticles.transform.position = new Vector3(0, 1, 5);
            obstacleParticles.Play();
        }

        IsRunning = false;
        IsGameOver = true;
        Time.timeScale = 0f;

        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt(BestKey, best);
            PlayerPrefs.Save();
        }

        if (gameOverText)
            gameOverText.text = $"Score: {score}\nBest: {best}";

        if (gameOverPanel) gameOverPanel.SetActive(true);
        UpdateUI();
    }

    void ShowStart()
    {
        if (startPanel) startPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = score.ToString();
        if (coinText) coinText.text = coins.ToString();
        if (bestText) bestText.text = best.ToString();

        if (boostText)
        {
            bool boosted = FactBoostRemaining > 0f;
            boostText.gameObject.SetActive(boosted);
            if (boosted) boostText.text = $"Fact boost {FactBoostRemaining:0.0}s";
        }

        if (factText && factText.gameObject.activeSelf && factMessageTimer <= 0f)
            factText.gameObject.SetActive(false);
    }

    void PlaySound(AudioClip clip, float volume = 0.5f)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.PlayOneShot(clip, volume);
        }
    }
}