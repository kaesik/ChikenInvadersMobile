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

    #region Wobble
    [Header("Wobble")]
    public float wobbleAmplitudeX = 0.3f;
    public float wobbleAmplitudeZ = 0.2f;
    public float wobbleFrequency = 2f;
    #endregion

    private float _currentHealth;
    private Renderer _rend;
    private Color _originalColor;
    private Coroutine _shootCo;
    private Transform _player;
    private PlayerController _playerController;
    private bool _hitPlayer;

    private bool _charging;
    private bool _returning;
    private Vector3 _chargeTarget;
    private float _chargeSpeed;

    private Transform _homeParent;
    private Vector3 _homeLocalPos;

    private bool _exploded;

    private Vector3 _baseLocalPos;
    private float _wobbleOffset;

    private void Awake()
    {
        _rend = GetComponentInChildren<Renderer>();
        if (_rend != null) _originalColor = _rend.material.color;
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (!playerObj) return;
        _player = playerObj.transform;
        _playerController = playerObj.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _currentHealth = maxHealth;
        _hitPlayer = false;
        _charging = false;
        _returning = false;
        _exploded = false;

        if (_rend) _rend.material.color = _originalColor;
        if (enemyProjectilePrefab) _shootCo = StartCoroutine(ShootLoop());

        if (EnemyCharger.Instance) EnemyCharger.Instance.RegisterEnemy(this);

        _baseLocalPos = transform.localPosition;
        _wobbleOffset = Random.Range(0f, Mathf.PI * 2f);
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

            if (_player)
            {
                var distToPlayer = Vector3.Distance(transform.position, _player.position);
                if (distToPlayer <= touchDamageRadius)
                {
                    if (!_playerController && _player)
                        _playerController = _player.GetComponent<PlayerController>();

                    if (_playerController) _playerController.TakeDamage(1);

                    _hitPlayer = true;
                    SpawnExplosion();
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
                    if (transform.parent) _baseLocalPos = transform.localPosition;
                }
            }
            else
            {
                _returning = false;
            }

            return;
        }

        ApplyWobble();

        if (!_player) return;
        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist > touchDamageRadius) return;

        if (!_playerController && _player)
            _playerController = _player.GetComponent<PlayerController>();

        if (_playerController) _playerController.TakeDamage(1);

        _hitPlayer = true;
        SpawnExplosion();
        Destroy(gameObject);
    }

    private void ApplyWobble()
    {
        if (!transform.parent) return;

        var t = Time.time * wobbleFrequency + _wobbleOffset;
        var offset = new Vector3(Mathf.Sin(t) * wobbleAmplitudeX, 0f, Mathf.Cos(t) * wobbleAmplitudeZ);
        transform.localPosition = _baseLocalPos + offset;
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
            if (AudioManager.Instance) AudioManager.Instance.PlayEnemyShoot();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hitPlayer) return;

        var pc = other.GetComponent<PlayerController>();
        if (!other.CompareTag("Player") && !pc) return;
        if (pc == null) return;

        pc.TakeDamage(1);
        _hitPlayer = true;
        SpawnExplosion();
        Destroy(gameObject);
    }

    public void TakeDamage(float dmg)
    {
        _currentHealth -= dmg;
        if (_rend) StartCoroutine(FlashOnHit());
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
        if (EnemyCharger.Instance) EnemyCharger.Instance.UnregisterEnemy(this);

        SpawnExplosion();

        if (AudioManager.Instance) AudioManager.Instance.PlayChickenDie();
        
        Destroy(gameObject);

        if (chickenWingPrefab)
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
