using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    [Header("FX")]
    public GameObject floatingTextPrefab;
    #endregion

    #region Refs
    [Header("Refs")]
    public EnemySpawner spawner;
    #endregion

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);

        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        var hud = HUDController.Instance;
        if (!hud) return;
        hud.SetScore(score);
        hud.SetLives(lives);
        hud.SetWave(wave);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
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
    
    public void ShowFloatingText(string text, Vector3 worldPos)
    {
        if (!floatingTextPrefab) return;

        var canvas = HUDController.Instance ? HUDController.Instance.GetComponentInParent<Canvas>() : null;
        if (!canvas) return;

        if (!Camera.main) return;
        var screenPos = Camera.main.WorldToScreenPoint(worldPos);
        var go = Instantiate(floatingTextPrefab, canvas.transform);
        go.transform.position = screenPos;

        var ft = go.GetComponent<FloatingText>();
        if (ft) ft.Show(text);
    }
    
    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}