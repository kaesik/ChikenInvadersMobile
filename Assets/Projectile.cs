using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 18f;
    public float lifeTime = 3f;
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.forward * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // trafiamy wroga?
        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}