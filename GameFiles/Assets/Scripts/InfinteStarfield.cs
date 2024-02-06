using UnityEngine;

public class InfiniteStarfield : MonoBehaviour
{
    private Transform tx;
    private ParticleSystem.Particle[] points;
    private Vector3[] directions;
    private ParticleSystem particleSystem;

    public int starsMax = 1000;
    public float starSize = .05f;
    public float starDistance = 100;
    public float starClipDistance = 1;
    private float starDistanceSqr;
    private float starClipDistanceSqr;

    public float movementSpeed = 1.1f;
    public float acceleration = 0.1f;

    void Start()
    {
        tx = transform;
        starDistanceSqr = starDistance * starDistance;
        starClipDistanceSqr = starClipDistance * starClipDistance;
        particleSystem = GetComponent<ParticleSystem>();
        directions = new Vector3[starsMax];
        CreateStars();
    }

    private void CreateStars()
    {
        points = new ParticleSystem.Particle[starsMax];

        for (int i = 0; i < starsMax; i++)
        {
            points[i].position = Random.insideUnitSphere * starDistance + tx.position;
            points[i].startColor = new Color(1, 1, 1, 1);
            points[i].startSize = starSize;
            directions[i] = GetSkewedDirection();
        }
    }

    private Vector3 GetSkewedDirection()
    {
        Vector3 direction = Random.insideUnitSphere;
        direction.z -= 1;
        return direction.normalized;
    }

    void Update()
    {
        movementSpeed += acceleration * Time.deltaTime;

        for (int i = 0; i < starsMax; i++)
        {
            points[i].position += directions[i] * movementSpeed * Time.deltaTime;

            if ((points[i].position - tx.position).sqrMagnitude > starDistanceSqr)
            {
                points[i].position = Random.insideUnitSphere.normalized * starDistance + tx.position;
                directions[i] = GetSkewedDirection();
            }

            if ((points[i].position - tx.position).sqrMagnitude <= starClipDistanceSqr)
            {
                float percent = (points[i].position - tx.position).sqrMagnitude / starClipDistanceSqr;
                points[i].startColor = new Color(1, 1, 1, percent);
                points[i].startSize = percent * starSize;
            }
        }

        particleSystem.SetParticles(points, points.Length);
    }
}
