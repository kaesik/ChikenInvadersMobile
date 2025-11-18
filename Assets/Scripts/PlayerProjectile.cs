using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    #region Stats
    [Header("Motion")]
    public float speed = 20f;
    public float lifeTime = 3f;
    public float topZLimit = 10f;

    [Header("Damage")]
    public float damage = 1f;
    #endregion

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (!_rb) _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.velocity = Vector3.forward * speed;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (transform.position.z > topZLimit)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemyProjectile = other.GetComponent<EnemyProjectile>();
        if (enemyProjectile)
        {
            Destroy(enemyProjectile.gameObject);
            Destroy(gameObject);
            return;
        }

        var enemy = other.GetComponent<Enemy>();
        if (!enemy) return;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}