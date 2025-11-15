using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Stats
    [Header("Motion")]
    public float speed = 18f;
    public float lifeTime = 3f;
    public float topZLimit = 10f;

    [Header("Damage")]
    public int damage = 1;
    #endregion

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.forward * (speed * Time.deltaTime);
        if (transform.position.z > topZLimit) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}