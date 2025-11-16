using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float hitRadius = 0.9f;
    public GameObject explosionPrefab;
    public float explosionLifetime = 2f;

    private Transform _player;
    private bool _hit;

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_hit) return;
        if (!_player) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist > hitRadius) return;

        _hit = true;

        var pc = _player.GetComponent<PlayerController>();
        if (pc) pc.TakeDamage(1);

        if (explosionPrefab)
        {
            var fx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (explosionLifetime > 0f) Destroy(fx, explosionLifetime);
        }
        
        if (AudioManager.Instance) AudioManager.Instance.PlayMeteorDestroy();
        
        Destroy(gameObject);
    }
}