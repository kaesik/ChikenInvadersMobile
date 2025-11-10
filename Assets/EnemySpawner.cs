using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int columns = 6;
    public int rows = 3;
    public float spacingX = 1.2f;
    public float spacingZ = 1.0f;

    public float moveSpeed = 1.0f; // w stronę gracza (–Z kamery? tutaj +Z idzie od kamery, my stoimy na -Z, więc wrogowie zbliżają się zmniejszając dystans)
    public float startZ = 10f;
    public float endZ = -6.5f; // linia porażki

    private readonly List<GameObject> _alive = new List<GameObject>();
    private bool _waveActive;

    private void Start()
    {
        SpawnWave();
    }

    private void Update()
    {
        if (!_waveActive) return;
        // całą grupę zbliżamy przesuwając spawner
        transform.position += Vector3.back * (moveSpeed * Time.deltaTime);

        // przegrana jeśli dotarli
        if (transform.position.z <= endZ)
        {
            _waveActive = false;
            GameManager.Instance.LoseLife();
        }

        // jeśli wszyscy zniszczeni – nowa fala
        _alive.RemoveAll(e => !e);
        if (_alive.Count != 0) return;
        GameManager.Instance.NextWave();
        SpawnWave();
    }

    public void SpawnWave()
    {
        // Reset pozycji spawnera przed graczem
        transform.position = new Vector3(0f, 0f, startZ);

        _alive.Clear();
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < columns; c++)
            {
                var localPos = new Vector3(
                    (c - (columns - 1) / 2f) * spacingX,
                    0.3f,
                    -(r * spacingZ)
                );
                var enemy = Instantiate(enemyPrefab, transform.position + localPos, Quaternion.identity, transform);
                _alive.Add(enemy);
            }
        }

        _waveActive = true;
    }
}