using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    #region Motion
    [Header("Motion")]
    public float speed = 12f;
    public float lifeTime = 4f;
    public float maxAngleOffset = 5f;
    #endregion

    #region Player Hit
    [Header("Player Hit")]
    public float hitRadius = 0.75f;
    #endregion

    private bool _hit;
    private Vector3 _direction;
    private Transform _player;

    private void Awake()
    {
        var angle = Random.Range(-maxAngleOffset, maxAngleOffset);
        var rot = Quaternion.Euler(0f, angle, 0f);
        _direction = rot * Vector3.back;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_hit) return;

        transform.position += _direction * (speed * Time.deltaTime);

        if (!_player) return;

        if (Vector3.Distance(transform.position, _player.position) <= hitRadius)
        {
            HitPlayer();
            return;
        }

        CheckHitPlayerProjectile();
    }

    private void CheckHitPlayerProjectile()
    {
        if (_hit) return;

        var projectiles = FindObjectsOfType<Projectile>();

        foreach (var p in projectiles)
        {
            var dist = Vector3.Distance(transform.position, p.transform.position);
            if (!(dist < 0.4f)) continue;
            Destroy(p.gameObject);
            Destroy(gameObject);
            _hit = true;
            return;
        }
    }

    private void HitPlayer()
    {
        if (_hit) return;

        _hit = true;

        if (_player)
        {
            var pc = _player.GetComponent<PlayerController>();
            if (pc) pc.TakeDamage();
        }

        Destroy(gameObject);
    }
}