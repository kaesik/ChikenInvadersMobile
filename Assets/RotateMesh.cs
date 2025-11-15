using UnityEngine;

public class RotateMesh : MonoBehaviour
{
    public Vector3 rotationAxis = new Vector3(0f, 1f, 0f);
    public float rotationSpeed = 90f;
    public bool useWorldSpace = false;
    public bool randomizeSpeed = false;
    public float randomMin = 60f;
    public float randomMax = 180f;

    private void Start()
    {
        if (randomizeSpeed)
        {
            rotationSpeed = Random.Range(randomMin, randomMax);
        }
    }

    private void Update()
    {
        var delta = rotationAxis.normalized * (rotationSpeed * Time.deltaTime);
        transform.Rotate(delta, useWorldSpace ? Space.World : Space.Self);
    }
}