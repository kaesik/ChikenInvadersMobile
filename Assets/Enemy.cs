using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public float maxHealth = 2f;
    public GameObject chickenWingPrefab;

    [Header("Powerup")]
    public GameObject shotUpgradePrefab;
    public float shotUpgradeChance = 0.15f;
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

    #region Charge
    [Header("Charge")]
    public float returnPositionTolerance = 0.1f;
    #endregion

    #region Explosion
    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionLifetime = 2f;
    #endregion

    private float _currentHealth;
    private Renderer _rend;
    private Color _originalColor;
    private Coroutine _shootCo;
    private Transform _player;
    private GameManager _gm;
    private bool _hitPlayer;

    private bool _charging;
    private bool _returning;
    private Vector3 _chargeTarget;
    private float _chargeSpeed;

    private Transform _homeParent;
    private Vector3 _homeLocalPos;

    private bool _exploded;

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
        _hitPlayer = false;
        _charging = false;
        _returning = false;
        _exploded = false;

        if (_rend != null) _rend.material.color = _originalColor;
        if (enemyProjectilePrefab != null) _shootCo = StartCoroutine(ShootLoop());

        if (EnemyCharger.Instance != null) EnemyCharger.Instance.RegisterEnemy(this);
    }

    private void OnDisable()
    {
        if (_shootCo != null) StopCoroutine(_shootCo);
        if (EnemyCharger.Instance != null) EnemyCharger.Instance.UnregisterEnemy(this);
    }

    private void Update()
    {
        if (_hitPlayer) return;

        if (_charging)
        {
            transform.position = Vector3.MoveTowards(transform.position, _chargeTarget, _chargeSpeed * Time.deltaTime);

            if (_player && _gm)
            {
                var distToPlayer = Vector3.Distance(transform.position, _player.position);
                if (distToPlayer <= touchDamageRadius)
                {
                    _hitPlayer = true;
                    SpawnExplosion();
                    _gm.LoseLife();
                    Destroy(gameObject);
                    return;
                }
            }

            var distToTarget = Vector3.Distance(transform.position, _chargeTarget);
            if (!(distToTarget <= returnPositionTolerance)) return;
            _charging = false;
            _returning = true;

            return;
        }

        if (_returning)
        {
            if (_homeParent)
            {
                var targetPos = _homeParent.TransformPoint(_homeLocalPos);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _chargeSpeed * Time.deltaTime);
                var distBack = Vector3.Distance(transform.position, targetPos);
                if (distBack <= returnPositionTolerance)
                {
                    _returning = false;
                }
            }
            else
            {
                _returning = false;
            }

            return;
        }

        if (!_player || !_gm) return;
        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist > touchDamageRadius) return;
        _hitPlayer = true;
        SpawnExplosion();
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
        SpawnExplosion();
        _gm.LoseLife();
        Destroy(gameObject);
    }

    public void TakeDamage(float dmg)
    {
        _currentHealth -= dmg;
        if (_rend != null) StartCoroutine(FlashOnHit());
        if (_currentHealth <= 0f) Die();
    }

    public void StartCharge(Vector3 playerPosition, float speed)
    {
        if (_charging || _hitPlayer) return;
        _homeParent = transform.parent;
        _homeLocalPos = transform.localPosition;
        _chargeTarget = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        _chargeSpeed = speed;
        _charging = true;
        _returning = false;
    }

    private IEnumerator FlashOnHit()
    {
        _rend.material.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        _rend.material.color = _originalColor;
    }

    private void Die()
    {
        if (EnemyCharger.Instance != null) EnemyCharger.Instance.UnregisterEnemy(this);

        SpawnExplosion();

        Destroy(gameObject);

        if (chickenWingPrefab != null)
            Instantiate(chickenWingPrefab, transform.position, Quaternion.identity);

        if (shotUpgradePrefab && Random.value < shotUpgradeChance)
            Instantiate(shotUpgradePrefab, transform.position, Quaternion.identity);
    }

    private void SpawnExplosion()
    {
        if (_exploded) return;
        _exploded = true;
        if (!explosionPrefab) return;
        var fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        if (explosionLifetime > 0f) Destroy(fx, explosionLifetime);
    }
}
