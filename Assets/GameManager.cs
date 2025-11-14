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
            spawner.SpawnWave();
        }
    }

    public void NextWave()
    {
        _wave++;
        spawner.moveSpeed += 0.2f;
        if (_wave % 2 == 0) spawner.rows = Mathf.Min(spawner.rows + 1, 5);
        if (_wave % 3 == 0) spawner.columns = Mathf.Min(spawner.columns + 1, 8);
    }

    private void GameOver()
    {
        _gameOver = true;
        gameOverPanel.SetActive(true);
        player.enabled = false;
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (livesText) livesText.text = "Lives: " + lives;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}