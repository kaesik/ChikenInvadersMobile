using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public int maxHealth = 2;
    public GameObject chickenWingPrefab;
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

    #region Player Hit
    [Header("Player Hit")]
    public float touchDamageRadius = 0.75f;
    #endregion

    private int _currentHealth;
    private Renderer _rend;
    private Color _originalColor;
    private Coroutine _shootCo;
    private Transform _player;
    private GameManager _gm;
    private bool _hitPlayer;

    private void Awake()
    {
        _rend = GetComponentInChildren<Renderer>();
        if (_rend != null) _originalColor = _rend.material.color;
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
        _gm = GameManager.Instance ?? FindObjectOfType<GameManager>();
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

    private void Update()
    {
        if (_hitPlayer) return;
        if (!_player || !_gm) return;
        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist > touchDamageRadius) return;
        _hitPlayer = true;
        _gm.LoseLife();
        Destroy(gameObject);
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

    private void OnTriggerEnter(Collider other)
    {
        if (_hitPlayer) return;
        if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null) return;
        if (_gm == null) _gm = GameManager.Instance ?? FindObjectOfType<GameManager>();
        if (_gm == null) return;
        _hitPlayer = true;
        _gm.LoseLife();
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
        Destroy(gameObject);
        if (chickenWingPrefab != null)
        {
            Instantiate(chickenWingPrefab, transform.position, Quaternion.identity);
        }
    }
}
