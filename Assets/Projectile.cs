using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 18f;
    public float lifeTime = 3f;
    public int damage = 1;
    public float topZLimit = 10f;

    private Rigidbody _rb;
    private bool _hit;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void Start() => Destroy(gameObject, lifeTime);

    private void FixedUpdate()
    {
        if (_hit) return;

        var next = _rb.position + Vector3.forward * (speed * Time.fixedDeltaTime);
        _rb.MovePosition(next);

        if (next.z > topZLimit)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hit) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        _hit = true;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}