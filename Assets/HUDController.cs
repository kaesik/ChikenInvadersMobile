using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("Score")]
    public TextMeshProUGUI scoreText;

    [Header("Lives")]
    public GameObject[] lifeHearts;

    [Header("Wave")]
    public TextMeshProUGUI waveText;

    private int _score;
    private int _lives;
    private int _wave;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetLives(3);
    }

    public void SetScore(int value)
    {
        _score = value;
        RefreshScore();
    }

    public void AddScore(int value)
    {
        _score += value;
        RefreshScore();
    }

    public void SetLives(int value)
    {
        _lives = lifeHearts is { Length: > 0 } ? Mathf.Clamp(value, 0, lifeHearts.Length) : value;

        RefreshLives();
    }

    public void SetWave(int value)
    {
        _wave = value;
        RefreshWave();
    }

    private void RefreshScore()
    {
        if (!scoreText) return;
        scoreText.text = _score.ToString();
    }

    private void RefreshLives()
    {
        if (lifeHearts == null || lifeHearts.Length == 0) return;

        for (var i = 0; i < lifeHearts.Length; i++)
        {
            if (!lifeHearts[i]) continue;
            lifeHearts[i].SetActive(i < _lives);
        }
    }

    private void RefreshWave()
    {
        if (!waveText) return;
        waveText.text = "Wave " + _wave;
    }
}