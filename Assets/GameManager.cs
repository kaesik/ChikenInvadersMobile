using UnityEngine;

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
    public GameObject gameOverPanel;
    #endregion

    #region Refs
    [Header("Refs")]
    public EnemySpawner spawner;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        var hud = HUDController.Instance;
        if (hud == null) return;
        hud.SetScore(score);
        hud.SetLives(lives);
        hud.SetWave(wave);
    }

    public void AddScore(int value)
    {
        score += value;

        var hud = HUDController.Instance;
        if (hud) hud.SetScore(score);
    }

    public void LoseLife()
    {
        lives -= 1;

        var hud = HUDController.Instance;
        if (hud) hud.SetLives(lives);

        if (lives <= 0) GameOver();
    }

    public void NextWave()
    {
        wave += 1;

        var hud = HUDController.Instance;
        if (hud) hud.SetWave(wave);

        if (!spawner) return;
        spawner.rows = Mathf.Clamp(spawner.rows + 1, 1, 8);
        spawner.horizontalSpeed += 0.15f;
    }

    private void GameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}