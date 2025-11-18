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
    public float maxZ = 1.5f;
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

    private bool _mouseDown;
    private Vector2 _lastMousePos;

    private bool _movedThisFrame;

    private void Start()
    {
        _cam = Camera.main;
        if (_cam) _camInitialLocalPos = _cam.transform.localPosition;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        
        _movedThisFrame = false;

        HandleMove();

        if (!_movedThisFrame)
            ApplyTilt(Vector3.zero);

        HandleShoot();
    }

    private void HandleMove()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Moved && touch.phase != TouchPhase.Stationary) return;
            var delta = touch.deltaPosition;
            MoveByDelta(delta);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDown = true;
                _lastMousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _mouseDown = false;
            }

            if (!_mouseDown || !Input.GetMouseButton(0)) return;
            var current = (Vector2)Input.mousePosition;
            var delta = current - _lastMousePos;
            _lastMousePos = current;
            MoveByDelta(delta);
        }
    }

    private void MoveByDelta(Vector2 delta)
    {
        if (delta.sqrMagnitude <= Mathf.Epsilon) return;

        _movedThisFrame = true;

        var before = transform.position;

        var move = new Vector3(delta.x, 0f, delta.y);
        move *= moveSpeed / Screen.height;

        var newPos = before + move;
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
        var proj = go.GetComponent<PlayerProjectile>();
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
        if (Time.timeScale <= 0f) return;
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
