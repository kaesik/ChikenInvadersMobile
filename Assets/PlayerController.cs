using UnityEngine;

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

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
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
        var newPos = Vector3.MoveTowards(transform.position, targetXZ, moveSpeed * Time.deltaTime);

        newPos.x = Mathf.Clamp(newPos.x, -clampX, clampX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        transform.position = newPos;
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

        if (amount == 1)
        {
            SpawnProjectile(shootPoint.position, shootPoint.rotation);
            return;
        }

        if (amount == 2)
        {
            SpawnProjectile(OffsetPos(-horizontalSpread), shootPoint.rotation);
            SpawnProjectile(OffsetPos(horizontalSpread), shootPoint.rotation);
            return;
        }

        if (amount == 3)
        {
            SpawnProjectile(OffsetPos(-horizontalSpread), shootPoint.rotation);
            SpawnProjectile(shootPoint.position, shootPoint.rotation);
            SpawnProjectile(OffsetPos(horizontalSpread), shootPoint.rotation);
            return;
        }

        if (amount == 4)
        {
            SpawnProjectile(OffsetPos(-horizontalSpread * 0.6f), shootPoint.rotation);
            SpawnProjectile(OffsetPos(horizontalSpread * 0.6f), shootPoint.rotation);

            SpawnProjectile(OffsetPos(-horizontalSpread), RotAngle(-angleSpread));
            SpawnProjectile(OffsetPos(horizontalSpread), RotAngle(angleSpread));
            return;
        }

        if (amount == 5)
        {
            SpawnProjectile(OffsetPos(-horizontalSpread * 0.6f), shootPoint.rotation);
            SpawnProjectile(shootPoint.position, shootPoint.rotation);
            SpawnProjectile(OffsetPos(horizontalSpread * 0.6f), shootPoint.rotation);

            SpawnProjectile(OffsetPos(-horizontalSpread), RotAngle(-angleSpread));
            SpawnProjectile(OffsetPos(horizontalSpread), RotAngle(angleSpread));
            return;
        }

        if (amount == 6)
        {
            SpawnProjectile(OffsetPos(-horizontalSpread * 0.4f), shootPoint.rotation);
            SpawnProjectile(OffsetPos(horizontalSpread * 0.4f), shootPoint.rotation);

            SpawnProjectile(OffsetPos(-horizontalSpread * 0.9f), RotAngle(-angleSpread));
            SpawnProjectile(OffsetPos(horizontalSpread * 0.9f), RotAngle(angleSpread));
            SpawnProjectile(OffsetPos(-horizontalSpread * 1.4f), RotAngle(-angleSpread * 1.5f));
            SpawnProjectile(OffsetPos(horizontalSpread * 1.4f), RotAngle(angleSpread * 1.5f));
            return;
        }

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
        if (proj != null) proj.damage = currentDamage;
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
}
