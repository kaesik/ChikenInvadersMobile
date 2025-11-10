using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 12f;
    public float clampX = 4.5f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float fireRate = 6f; // strzałów na sekundę
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
        // Obsługa dotyku i myszy – przesuwanie w poziomie
        if (Input.touchCount > 0)
        {
            var world = ScreenToWorldX(Input.GetTouch(0).position);
            MoveTowardsX(world.x);
        }
        else if (Input.GetMouseButton(0))
        {
            var world = ScreenToWorldX(Input.mousePosition);
            MoveTowardsX(world.x);
        }
    }

    private Vector3 ScreenToWorldX(Vector3 screenPos)
    {
        // rzutujemy na płaszczyznę Z gracza
        var zDistance = Mathf.Abs(_cam.transform.position.z - transform.position.z);
        var world = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
        return world;
    }

    private void MoveTowardsX(float targetX)
    {
        var newX = Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime);
        newX = Mathf.Clamp(newX, -clampX, clampX);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
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