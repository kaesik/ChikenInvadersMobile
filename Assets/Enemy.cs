using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public int maxHealth = 2;
    public int scoreValue = 10;
    #endregion

    #region Hit Feedback
    [Header("Hit Feedback")]
    public Color hitColor = Color.red;
    public float hitFlashDuration = 0.15f;
    #endregion

    #region Shooting
    [Header("Shooting")]
    public GameObject enemyProjectilePrefab;
    public Transform shootPoint;
    public float minShootDelay = 1.5f;
    public float maxShootDelay = 4.0f;
    public float shootChanceAlive = 0.65f;
    #endregion

    private int _currentHealth;
    private Renderer _rend;
    private Color _originalColor;
    private Coroutine _shootCo;

    private void Awake()
    {
        _rend = GetComponentInChildren<Renderer>();
        if (_rend != null) _originalColor = _rend.material.color;
    }

    private void OnEnable()
    {
        _currentHealth = maxHealth;
        if (_rend != null) _rend.material.color = _originalColor;
        if (enemyProjectilePrefab != null) _shootCo = StartCoroutine(ShootLoop());
    }

    private void OnDisable()
    {
        if (_shootCo != null) StopCoroutine(_shootCo);
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minShootDelay, maxShootDelay));
            if (Random.value > shootChanceAlive) continue;
            var origin = shootPoint ? shootPoint.position : transform.position;
            var rot = Quaternion.LookRotation(Vector3.back, Vector3.up);
            Instantiate(enemyProjectilePrefab, origin, rot);
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        if (!c.collider.CompareTag("Player")) return;
        GameManager.Instance.LoseLife();
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        if (_rend != null) StartCoroutine(FlashOnHit());
        if (_currentHealth <= 0) Die();
    }

    private IEnumerator FlashOnHit()
    {
        _rend.material.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        _rend.material.color = _originalColor;
    }

    private void Die()
    {
        GameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
