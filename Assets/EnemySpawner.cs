using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    #region Grid
    [Header("Prefab & Grid")]
    public GameObject enemyPrefab;
    public int columns = 6;
    public int rows = 3;
    public float spacingX = 1.2f;
    public float spacingZ = 1.0f;
    #endregion

    #region Formation
    [Header("Formation Position")]
    public float spawnZ = 10f;
    public float targetZ = 4.5f;
    public float descendSpeed = 6f;
    #endregion

    #region Movement
    [Header("Horizontal Movement")]
    public float horizontalSpeed = 3.0f;
    public float leftBoundX = -5.5f;
    public float rightBoundX = 5.5f;
    public float stepDownOnTurn = 0.35f;
    public float minZ = 2.5f;
    public float maxZ = 6.5f;
    #endregion

    private readonly List<GameObject> _alive = new();
    private bool _settled;
    private int _dir = 1;
    private float _halfWidth;
    private float _targetZInternal;

    private void Start()
    {
        SpawnWave();
    }

    private void FixedUpdate()
    {
        if (!_settled)
        {
            var pos = transform.position;
            var targetPos = new Vector3(pos.x, pos.y, _targetZInternal);
            pos = Vector3.MoveTowards(pos, targetPos, descendSpeed * Time.fixedDeltaTime);
            transform.position = pos;
            if (Mathf.Abs(pos.z - _targetZInternal) < 0.01f) _settled = true;
            return;
        }

        var posMove = transform.position;
        posMove.x += _dir * horizontalSpeed * Time.fixedDeltaTime;
        posMove.z = Mathf.MoveTowards(posMove.z, _targetZInternal, descendSpeed * Time.fixedDeltaTime);
        transform.position = posMove;

        var leftEdge = transform.position.x - _halfWidth;
        var rightEdge = transform.position.x + _halfWidth;

        if (rightEdge > rightBoundX || leftEdge < leftBoundX)
        {
            _dir *= -1;
            _targetZInternal = Mathf.Clamp(_targetZInternal - stepDownOnTurn, minZ, maxZ);
        }

        _alive.RemoveAll(e => !e);
        if (_alive.Count != 0) return;
        GameManager.Instance.NextWave();
        SpawnWave();
    }

    private void SpawnWave()
    {
        transform.position = new Vector3(0f, 0f, spawnZ);
        _settled = false;
        _dir = 1;
        _alive.Clear();
        _halfWidth = ((columns - 1) * spacingX) * 0.5f;
        _targetZInternal = targetZ;

        var gm = GameManager.Instance;
        var healthBonus = 0f;
        if (gm != null && gm.wave > 0)
            healthBonus = gm.enemyHealthBonusPerWave * (gm.wave - 1);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < columns; c++)
            {
                var localPos = new Vector3((c - (columns - 1) / 2f) * spacingX, 0.3f, -(r * spacingZ));
                var enemy = Instantiate(enemyPrefab, transform.position + localPos, Quaternion.identity, transform);
                var enemyComp = enemy.GetComponent<Enemy>();
                if (enemyComp != null)
                    enemyComp.maxHealth += healthBonus;
                _alive.Add(enemy);
            }
        }
    }
}
