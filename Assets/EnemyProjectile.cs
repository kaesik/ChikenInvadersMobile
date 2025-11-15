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

    private Rigidbody _rb;
    private bool _hit;
    private Vector3 _direction;

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
    }

    private void FixedUpdate()
    {
        if (_hit) return;
        var next = _rb.position + _direction * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(next);
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
        _hit = true;
        GameManager.Instance.LoseLife();
        Destroy(gameObject);
    }
}