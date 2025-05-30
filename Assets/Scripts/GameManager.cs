using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private float initialAsteroidCount = 4;
    [SerializeField] private float asteroidSpawnInterval = 3f;
    [SerializeField] private float spawnDistance = 12f;
    
    [Header("References")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    
    private int score;
    private bool isGameOver;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        StartGame();
    }
    
    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
    
    public void StartGame()
    {
        score = 0;
        isGameOver = false;
        UpdateScoreUI();
        gameOverPanel.SetActive(false);
        
        // Spawn initial asteroids
        for (int i = 0; i < initialAsteroidCount; i++)
        {
            SpawnAsteroid();
        }
        
        // Start spawning asteroids periodically
        InvokeRepeating(nameof(SpawnAsteroid), asteroidSpawnInterval, asteroidSpawnInterval);
    }
    
    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        CancelInvoke(nameof(SpawnAsteroid));
    }
    
    /// <summary>
    /// Restarts the game by reloading the current scene.
    /// Can be called from UI buttons or other game events.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void PauseGame()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }
    
    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }
    
    private void SpawnAsteroid()
    {
        // Spawn asteroid at random position outside the screen
        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = spawnDir * spawnDistance;
        
        Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
    }
} 