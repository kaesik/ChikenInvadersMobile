using UnityEngine;

public class ChickenWing : MonoBehaviour
{
    #region Motion
    [Header("Motion")]
    public float fallSpeed = 5f;
    public Vector3 rotationSpeed = new Vector3(0f, 180f, 0f);
    public float lifeTime = 6f;
    #endregion

    #region Score
    [Header("Score")]
    public int scoreValue = 10;
    public float pickupRadius = 1f;
    #endregion

    private Transform _player;
    private GameManager _gm;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        _gm = GameManager.Instance;
        if (_gm == null) _gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        transform.position += Vector3.back * (fallSpeed * Time.deltaTime);
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);

        if (!_player || !_gm) return;

        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist > pickupRadius) return;

        _gm.AddScore(scoreValue);
        Destroy(gameObject);
    }
}