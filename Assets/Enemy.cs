using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
    public int scoreValue = 10;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionEnter(Collision c)
    {
        if (!c.collider.CompareTag("Player")) return;
        GameManager.Instance.LoseLife();
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}