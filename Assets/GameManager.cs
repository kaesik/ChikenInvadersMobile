using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    #endregion

    #region State
    [Header("State")]
    public int lives = 3;
    public int score = 0;
    #endregion

    #region Progression
    [Header("Progression")]
    public int wave = 1;
    public float enemyHealthBonusPerWave = 0.5f;
    #endregion

    #region UI
    [Header("UI")]
    public TMP_Text scoreTMP;
    public TMP_Text livesTMP;
    public GameObject gameOverPanel;
    #endregion

    #region Refs
    [Header("Refs")]
    public EnemySpawner spawner;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        UpdateScoreUI();
        UpdateLivesUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    public void LoseLife()
    {
        lives -= 1;
        UpdateLivesUI();
        if (lives <= 0) GameOver();
    }

    public void NextWave()
    {
        wave += 1;

        if (!spawner) return;
        spawner.rows = Mathf.Clamp(spawner.rows + 1, 1, 8);
        spawner.horizontalSpeed += 0.15f;
    }

    private void GameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void UpdateScoreUI()
    {
        if (scoreTMP) scoreTMP.text = "Score: " + score;
    }

    private void UpdateLivesUI()
    {
        if (livesTMP) livesTMP.text = "Lives: " + lives;
    }
}