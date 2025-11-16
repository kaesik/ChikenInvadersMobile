using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    #region Move
    [Header("Move")]
    public float moveSpeed = 12f;
    public float clampX = 4.5f;
    #endregion

    #region Bounds
    [Header("Bounds")]
    public float minZ = -6.0f;
    public float maxZ =  1.5f;
    #endregion

    #region Shooting
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float fireRate = 6f;
    private float _fireTimer;
    #endregion

    #region Shot Upgrades
    [Header("Shot Upgrades")]
    public int shotAmount = 1;
    public float currentDamage = 1f;
    public float damageStep = 0.5f;
    public float fireRateStep = 0.5f;
    public float horizontalSpread = 0.35f;
    public float angleSpread = 10f;
    public int maxShotAmount = 12;
    #endregion

    #region Hit Feedback
    [Header("Hit Feedback")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;
    #endregion

    #region Slow Motion
    [Header("Slow Motion")]
    public float slowTimeScale = 0.3f;
    public float slowDuration = 0.15f;
    #endregion
    
    #region Tilt
    [Header("Tilt")]
    public Transform model;           
    public float tiltAmountX = 15f;   
    public float tiltAmountZ = 10f;   
    public float tiltSmooth = 10f;    
    #endregion

    private Camera _cam;
    private Vector3 _camInitialLocalPos;
    private bool _isShaking;
    private bool _isInSlowMo;

    private void Start()
    {
        _cam = Camera.main;
        if (_cam) _camInitialLocalPos = _cam.transform.localPosition;
    }

    public void Update()
    {
        HandleMove();
        HandleShoot();
    }

    private void HandleMove()
    {
        if (Input.touchCount > 0)
        {
            var world = ScreenToWorldOnGround(Input.GetTouch(0).position);
            MoveTowardsXZ(world);
        }
        else if (Input.GetMouseButton(0))
        {
            var world = ScreenToWorldOnGround(Input.mousePosition);
            MoveTowardsXZ(world);
        }
    }

    private Vector3 ScreenToWorldOnGround(Vector2 screenPos)
    {
        var ray = _cam.ScreenPointToRay(screenPos);
        var ground = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
        if (!ground.Raycast(ray, out var enter)) return transform.position;
        var world = ray.GetPoint(enter);
        return world;
    }

    private void MoveTowardsXZ(Vector3 target)
    {
        var targetXZ = new Vector3(target.x, transform.position.y, target.z);

        var before = transform.position;

        var newPos = Vector3.MoveTowards(transform.position, targetXZ, moveSpeed * Time.deltaTime);
        newPos.x = Mathf.Clamp(newPos.x, -clampX, clampX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        transform.position = newPos;

        var velocity = (newPos - before) / Time.deltaTime;
        ApplyTilt(velocity);
    }
    
    private void HandleShoot()
    {
        _fireTimer += Time.deltaTime;
        var interval = 1f / fireRate;
        if (!(_fireTimer >= interval)) return;
        _fireTimer = 0f;
        if (!projectilePrefab || !shootPoint) return;

        FirePattern();
    }

    private void FirePattern()
    {
        var amount = Mathf.Max(1, shotAmount);
        var clamped = Mathf.Clamp(amount, 1, maxShotAmount);
        var middle = (clamped - 1) * 0.5f;
        for (var i = 0; i < clamped; i++)
        {
            var offsetIndex = i - middle;
            var pos = OffsetPos(offsetIndex * horizontalSpread * 0.6f);
            var angle = offsetIndex * angleSpread * 0.5f;
            var rot = RotAngle(angle);
            SpawnProjectile(pos, rot);
        }
        
        if (AudioManager.Instance) AudioManager.Instance.PlayPlayerShoot();
    }

    private Vector3 OffsetPos(float offsetX)
    {
        return shootPoint.position + shootPoint.right * offsetX;
    }

    private Quaternion RotAngle(float angle)
    {
        return shootPoint.rotation * Quaternion.Euler(0f, angle, 0f);
    }

    private void SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(projectilePrefab, position, rotation);
        var proj = go.GetComponent<Projectile>();
        if (proj) proj.damage = currentDamage;
    }

    public void UpgradeDamage()
    {
        currentDamage += damageStep;
    }

    public void UpgradeFireRate()
    {
        fireRate += fireRateStep;
    }

    public void UpgradeAmount()
    {
        shotAmount = Mathf.Clamp(shotAmount + 1, 1, maxShotAmount);
    }

    public void TakeDamage()
    {
        var gm = GameManager.Instance;
        if (gm) gm.LoseLife();
        if (AudioManager.Instance) AudioManager.Instance.PlayPlayerHit();
        if (_cam && !_isShaking) StartCoroutine(ShakeRoutine());
        gm.Vibrate();
        
        if (Time.timeScale > 0.01f)
            SlowMotionHit();
    }

    private void SlowMotionHit()
    {
        if (_isInSlowMo) return;
        StartCoroutine(SlowMotionRoutine());
    }

    private IEnumerator SlowMotionRoutine()
    {
        if (Time.timeScale <= 0f)
        {
            yield break;
        }

        _isInSlowMo = true;

        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(slowDuration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        _isInSlowMo = false;
    }

    private IEnumerator ShakeRoutine()
    {
        _isShaking = true;
        var elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            if (_cam)
            {
                var offset = Random.insideUnitSphere * shakeMagnitude;
                _cam.transform.localPosition = _camInitialLocalPos + offset;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_cam) _cam.transform.localPosition = _camInitialLocalPos;
        _isShaking = false;
    }
    
    private void ApplyTilt(Vector3 velocity)
    {
        if (Time.timeScale == 0f) return;
        if (!model) return;

        var targetTilt = Quaternion.Euler(
            velocity.z * tiltAmountZ,
            0f,
            -velocity.x * tiltAmountX
        );

        model.localRotation = Quaternion.Lerp(
            model.localRotation,
            targetTilt,
            Time.deltaTime * tiltSmooth
        );
    }
}
