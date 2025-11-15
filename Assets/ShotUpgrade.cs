using UnityEngine;

public class ShotUpgrade : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float lifeTime = 6f;
    public float pickupRadius = 1f;

    [Header("Chances")]
    public float damageChance = 0.5f;
    public float fireRateChance = 0.3f;

    private Transform _player;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;
    }

    private void Update()
    {
        transform.position += Vector3.back * (fallSpeed * Time.deltaTime);

        if (!_player) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist > pickupRadius) return;

        var pc = _player.GetComponent<PlayerController>();
        if (pc)
        {
            var r = Random.value;
            if (r < damageChance)
            {
                pc.UpgradeDamage();
            }
            else if (r < damageChance + fireRateChance)
            {
                pc.UpgradeFireRate();
            }
            else
            {
                pc.UpgradeAmount();
            }
        }

        Destroy(gameObject);
    }
}