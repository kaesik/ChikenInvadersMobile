using UnityEngine;
using System.Collections;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject[] meteors;
    public float spawnZ = 12f;
    public float minX = -7f;
    public float maxX = 7f;

    public float minScale = 0.6f;
    public float maxScale = 2.2f;

    public float fallSpeed = 18f;
    public int meteorCount = 15;

    public void StartMeteorRain()
    {
        StartCoroutine(RainRoutine());
    }

    private IEnumerator RainRoutine()
    {
        for (var i = 0; i < meteorCount; i++)
        {
            var prefab = meteors[Random.Range(0, meteors.Length)];
            var x = Random.Range(minX, maxX);
            var pos = new Vector3(x, 0f, spawnZ);
            var m = Instantiate(prefab, pos, Quaternion.identity);

            var scale = Random.Range(minScale, maxScale);
            m.transform.localScale = Vector3.one * scale;

            var rb = m.GetComponent<Rigidbody>();
            if (!rb) rb = m.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = Vector3.back * fallSpeed;

            Destroy(m, 6f);

            yield return new WaitForSeconds(0.15f);
        }
    }
}