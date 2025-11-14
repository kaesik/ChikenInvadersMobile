using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 2;
    private int _currentHealth;
    public int scoreValue = 10;

    private Renderer _rend;
    private Color _originalColor;
    public Color hitColor = Color.red;
    public float hitFlashDuration = 0.15f;

    private void Awake()
    {
        _rend = GetComponentInChildren<Renderer>();
        if (_rend != null)
            _originalColor = _rend.material.color;
    }

    private void OnEnable()
    {
        _currentHealth = maxHealth;
        if (_rend != null)
            _rend.material.color = _originalColor;
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

        if (_rend != null)
            StartCoroutine(FlashOnHit());

        if (_currentHealth <= 0)
        {
            Die();
        }
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
