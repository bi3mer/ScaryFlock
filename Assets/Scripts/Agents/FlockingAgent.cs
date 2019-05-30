using UnityEngine;

public abstract class FlockingAgent : MonoBehaviour
{
    private readonly float maxRadius = 1.5f;
    private readonly float maxWeight = 100;

    public static Vector2 WorldMin;
    public static Vector2 WorldMax;
    public static LayerMask PredatorMask;
    public static LayerMask FriendlyMask;

    private float maxAcceleration = 3;
    private float maxVelocity = 1;
    private float predatorMaxVelocity = 2;
    public bool isPredator = false;

    [Header("Radius")]
    public float CohesionRadius;
    public float SeparationRadius;
    public float AllignmentRadius;
    public float WanderRadius;
    public float AvoidRadius;

    [Header("Weight")]
    public float CohesionWeight;
    public float SeparationWeight;
    public float AllignmentWeight;
    public float WanderWeight;
    public float AvoidWeight;

    [Header("Smoothing Movement")]
    public float Jitter;
    public float WanderDistanceRadius;


    public Vector2 Velocity { get; private set; }
    public Vector2 Acceleration { get; private set; }

    private float RandomBinomial => Random.Range(0f, 1f) - Random.Range(0f, 1f);

    private Vector3 wanderTarget;

    private void Start()
    {
        if (isPredator)
        {
            maxVelocity = predatorMaxVelocity;
        }

        Velocity = new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
    }

    public void RandomizeWeights()
    {
        CohesionRadius = Random.Range(0, maxRadius);
        SeparationRadius = Random.Range(0, maxRadius);
        AllignmentRadius = Random.Range(0, maxRadius);
        WanderRadius = Random.Range(0, maxRadius);
        AvoidRadius = Random.Range(0, maxRadius);

        CohesionWeight = Random.Range(0, maxWeight);
        SeparationWeight = Random.Range(0, maxWeight);
        AllignmentWeight = Random.Range(0, maxWeight);
        WanderWeight = Random.Range(0, maxWeight);
        AvoidWeight = Random.Range(0, maxWeight);

        Jitter = Random.Range(0, maxWeight);
        WanderDistanceRadius = Random.Range(0, maxRadius);
    }

    public void OnUpdate()
    {
        // run flocking 
        Vector2 temp = Combine();
        Acceleration = Vector2.ClampMagnitude(temp, maxAcceleration);
        Velocity = Vector2.ClampMagnitude(Velocity + Acceleration * Time.deltaTime, maxVelocity);
        transform.position = transform.position + (Vector3)(Velocity * Time.deltaTime);

        // Keep agent in world bounds
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        if (x < WorldMin.x) transform.position = new Vector3(WorldMin.x, y, z);
        if (x > WorldMax.x) transform.position = new Vector3(WorldMax.x, y, z);
        if (y < WorldMin.y) transform.position = new Vector3(transform.position.x, WorldMin.y, z);
        if (y > WorldMax.y) transform.position = new Vector3(transform.position.x, WorldMax.y, z);

        // set agent alignment 
        if (Velocity.magnitude > 0)
        {
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    protected Vector2 Combine()
    {
        if (isPredator)
        {
            return CohesionWeight * Cohesion() +
                SeparationWeight * Separation() +
                AllignmentWeight * Allignment() +
                WanderWeight * Wander() +
                AvoidWeight * Avoid();
        }
        else
        {
            return CohesionWeight * Cohesion() +
                SeparationWeight * Separation() +
                AllignmentWeight * Allignment() +
                WanderWeight * Wander();
        }
    }

    protected Vector2 Cohesion()
    {
        Vector3 result = new Vector3();
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, CohesionRadius, FriendlyMask);

        if (neighbors.Length > 0)
        {
            int agentCount = 0;

            for (int i = 0; i < neighbors.Length; ++i)
            {
                result += neighbors[i].transform.position;
                ++agentCount;
            }

            if (agentCount > 0)
            {
                result /= agentCount;
            }

            result = result - transform.position;
            result.Normalize();
        }

        return result;
    }

    protected Vector2 Separation()
    {
        Vector2 result = new Vector3();
        Collider2D[] neighbors;

        if (isPredator)
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, SeparationRadius, PredatorMask);
        }
        else
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, SeparationRadius, FriendlyMask);
        }

        for (int i = 0; i < neighbors.Length; ++i)
        {
            Vector3 towardsMe = transform.position - neighbors[i].transform.position;

            if (towardsMe.magnitude > 0)
            {
                result = towardsMe.normalized / towardsMe.magnitude;
            }

            result.Normalize();
        }

        return result;
    }

    protected Vector2 Allignment()
    {
        Vector2 result = new Vector2();
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, SeparationRadius, FriendlyMask);

        if (neighbors.Length > 0)
        {
            for (int i = 0; i < neighbors.Length; ++i)
            {
                result += FlockManager.Instance.Get(neighbors[i].name).Velocity;
            }

            result.Normalize();
        }

        return result;
    }

    public Vector2 Flee(Vector3 targ)
    {
        Vector2 desiredVel = (transform.position - targ).normalized * maxVelocity;
        return desiredVel - Velocity;
    }

    public Vector2 Avoid()
    {
        Vector2 result = new Vector3();
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, AvoidRadius, PredatorMask);

        for (int i = 0; i < enemies.Length; ++i)
        {
            result += Flee(enemies[i].transform.position);
        }

        return result;
    }

    public Vector2 Wander()
    {
        float jitter = Jitter * Time.deltaTime;
        wanderTarget += new Vector3(RandomBinomial * jitter, RandomBinomial * jitter, 0);

        wanderTarget.Normalize();
        wanderTarget *= WanderRadius;
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(0, 0, WanderDistanceRadius);
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);

        return (targetInWorldSpace - transform.position).normalized;
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, CohesionRadius);
    //}

#endif
}
