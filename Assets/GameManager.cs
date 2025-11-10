using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public GameObject gameOverPanel;

    [Header("Game")]
    public int lives = 3;
    public int score = 0;
    public EnemySpawner spawner;
    public PlayerController player;

    private int _wave = 1;
    private bool _gameOver;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // niepotrzebne dla jednej sceny
    }

    private void Start()
    {
        UpdateUI();
        gameOverPanel.SetActive(false);
    }

    public void AddScore(int value)
    {
        if (_gameOver) return;
        score += value;
        UpdateUI();
    }

    public void LoseLife()
    {
        if (_gameOver) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            // nowa fala po stracie życia
            spawner.SpawnWave();
        }
    }

    public void NextWave()
    {
        _wave++;
        // lekkie podkręcenie trudności
        spawner.moveSpeed += 0.2f;
        // opcjonalnie: zwiększ rows/columns co 2 fale
        if (_wave % 2 == 0) spawner.rows = Mathf.Min(spawner.rows + 1, 5);
        if (_wave % 3 == 0) spawner.columns = Mathf.Min(spawner.columns + 1, 8);
    }

    private void GameOver()
    {
        _gameOver = true;
        gameOverPanel.SetActive(true);
        // Możesz też zatrzymać strzelanie:
        player.enabled = false;
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (livesText) livesText.text = "Lives: " + lives;
    }

    // przypnij do przycisku Restart
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}