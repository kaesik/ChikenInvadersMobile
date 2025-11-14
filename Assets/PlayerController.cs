using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 12f;
    public float clampX = 4.5f;
    
    [Header("Bounds")]
    public float minZ = -6.0f;
    public float maxZ =  1.5f;
    
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float fireRate = 6f;
    private float _fireTimer;

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
        if (projectilePrefab && shootPoint)
        {
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
    }
}