using UnityEngine;
using System.Collections.Generic;

public class EnemyCharger : MonoBehaviour
{
    #region Settings
    [Header("Delay")]
    public float minDelay = 3f;
    public float maxDelay = 7f;

    [Header("Speed")]
    public float minChargeSpeed = 10f;
    public float maxChargeSpeed = 16f;
    #endregion

    public static EnemyCharger Instance;

    private readonly List<Enemy> _enemies = new List<Enemy>();
    private Transform _player;
    private bool _running;

    private void Awake()
    {
        if (!Instance) Instance = this; else Destroy(gameObject);
    }

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
        _running = true;
        ScheduleNextCharge();
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemy) return;
        if (!_enemies.Contains(enemy)) _enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (!enemy) return;
        _enemies.Remove(enemy);
    }

    private void ScheduleNextCharge()
    {
        if (!_running) return;
        var delay = Random.Range(minDelay, maxDelay);
        Invoke(nameof(ExecuteCharge), delay);
    }

    private void ExecuteCharge()
    {
        if (!_running) return;

        if (!_player)
        {
            ScheduleNextCharge();
            return;
        }

        _enemies.RemoveAll(e => !e || !e.gameObject.activeInHierarchy);

        if (_enemies.Count > 0)
        {
            var index = Random.Range(0, _enemies.Count);
            var enemy = _enemies[index];
            if (enemy)
            {
                var speed = Random.Range(minChargeSpeed, maxChargeSpeed);
                enemy.StartCharge(_player.position, speed);
            }
        }

        ScheduleNextCharge();
    }
}