using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
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

    private Rigidbody _rb;
    private bool _hit;
    private Vector3 _direction;
    private Transform _player;
    private GameManager _gm;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var angle = Random.Range(-maxAngleOffset, maxAngleOffset);
        var rot = Quaternion.Euler(0f, angle, 0f);
        _direction = rot * Vector3.back;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        _gm = GameManager.Instance ?? FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (_hit) return;

        var next = _rb.position + _direction * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(next);

        if (!_player || !_gm) return;
        var dist = Vector3.Distance(_rb.position, _player.position);
        if (dist > hitRadius) return;

        _hit = true;
        _gm.LoseLife();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hit) return;

        var playerProjectile = other.GetComponent<Projectile>();
        if (playerProjectile != null)
        {
            _hit = true;
            Destroy(playerProjectile.gameObject);
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null) return;
        if (_gm == null) _gm = GameManager.Instance ?? FindObjectOfType<GameManager>();
        if (_gm == null) return;
        _hit = true;
        _gm.LoseLife();
        Destroy(gameObject);
    }
}
