using System.Collections.Generic;

using UnityEngine.Assertions;
using UnityEngine;

public abstract class FlockingAgent : MonoBehaviour
{
    public const int ParameterSpace = 7;

    private readonly float maxRadius = 0.8f;
    private readonly float maxWeight = 100;

    public static Vector2 WorldMin;
    public static Vector2 WorldMax;
    public static LayerMask PredatorMask;
    public static LayerMask FriendlyMask;
    public static LayerMask AllMask;

    private float maxAcceleration = 3;
    private float maxVelocity = 1;
    private float predatorMaxVelocity = 2;
    public bool isPredator = false;
    private float updateOnFrameDivisibleBy;

    [Header("Radius")]
    public float SearchRadius;

    [Header("Weight")]
    public float CohesionWeight;
    public float SeparationWeight;
    public float AllignmentWeight;
    public float WanderWeight;
    public float AvoidWeight;

    [Header("Smoothing Movement")]
    public float Jitter;

    public Vector2 Velocity { get; private set; }
    public Vector2 Acceleration { get; protected set; }
    private Vector3 wanderTarget;

    public float MaxRadius => maxRadius;
    public float MaxWeight => maxWeight;

    public float[] Weights => new float[] {
        SearchRadius,
        CohesionWeight,
        SeparationWeight,
        AllignmentWeight,
        WanderWeight,
        AvoidWeight,
        Jitter
    };

    private void Awake()
    {
        if (isPredator)
        {
            updateOnFrameDivisibleBy = 1;
        }
        else
        {
            updateOnFrameDivisibleBy = Random.Range(1, 5);
        }
    }

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
        SearchRadius = Random.Range(0, maxRadius);

        CohesionWeight = Random.Range(0, maxWeight);
        SeparationWeight = Random.Range(0, maxWeight);
        AllignmentWeight = Random.Range(0, maxWeight);
        WanderWeight = Random.Range(0, maxWeight);
        AvoidWeight = Random.Range(0, maxWeight);
        Jitter = Random.Range(0, maxWeight);
    }

    public void UpdateWeights(float[] newWeights)
    {
        Assert.IsNotNull(newWeights);
        Assert.IsTrue(newWeights.Length == ParameterSpace);

        SearchRadius = newWeights[0];
        CohesionWeight = newWeights[1];
        SeparationWeight = newWeights[2];
        AllignmentWeight = newWeights[3];
        WanderWeight = newWeights[4];
        AvoidWeight = newWeights[5];
        Jitter = newWeights[6];
    }

    protected void KeepInBounds()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        if (x < WorldMin.x) transform.position = new Vector3(WorldMin.x, y, z);
        if (x > WorldMax.x) transform.position = new Vector3(WorldMax.x, y, z);
        if (y < WorldMin.y) transform.position = new Vector3(transform.position.x, WorldMin.y, z);
        if (y > WorldMax.y) transform.position = new Vector3(transform.position.x, WorldMax.y, z);
    }

    public void OnUpdate()
    {
        // optimzation problems so lets stop the agents from being able to update every frame and 
        // do a lot of work every frame
        if (Time.frameCount % updateOnFrameDivisibleBy != 0) return;

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, SearchRadius, AllMask);

        List<Vector3> enemyPositions = new List<Vector3>();
        List<Vector3> friendPositions = new List<Vector3>();
        List<Vector2> friendVelocities = new List<Vector2>();

        FlockManager flock = FlockManager.Instance;
        Collider2D neighbor;
        for (int i = 0; i < neighbors.Length; ++i)
        {
            neighbor = neighbors[i];

            if (neighbor.tag.Equals(Tag.Predator))
            {
                enemyPositions.Add(neighbor.transform.position);
            }
            else
            {
                friendPositions.Add(neighbor.transform.position);
                friendVelocities.Add(flock.Get(neighbor.name).Velocity);
            }
        }

        Vector2 temp = Acceleration + Combine(enemyPositions, friendPositions, friendVelocities);
        Acceleration = Vector2.ClampMagnitude(temp, maxAcceleration);
        Velocity = Vector2.ClampMagnitude(Velocity + Acceleration * Time.deltaTime, maxVelocity);
        transform.position = transform.position + (Vector3)(Velocity * Time.deltaTime);
        KeepInBounds();

        // set agent alignment 
        if (Velocity.magnitude > 0)
        {
            float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private float RandomBinomial => Random.Range(0f, 1f) - Random.Range(0f, 1f);

    private Vector2 Combine(List<Vector3> enemyPositions, List<Vector3> friendPositions, List<Vector2> friendVelocities)
    {
        if (!isPredator)
        {
            return CohesionWeight * Cohesion(friendPositions) +
                SeparationWeight * Separation(enemyPositions, friendPositions) +
                AllignmentWeight * Allignment(friendVelocities) +
                AvoidWeight * Avoid(enemyPositions);
        }
        else
        {
            return CohesionWeight * Cohesion(friendPositions) +
                SeparationWeight * Separation(enemyPositions, friendPositions) +
                AllignmentWeight * Allignment(friendVelocities);
        }
    }

    private Vector2 Wander()
    {
        float jitter = Weights[11] * Time.deltaTime;
        wanderTarget += new Vector3(RandomBinomial * jitter, RandomBinomial * jitter, 0);

        wanderTarget.Normalize();
        wanderTarget *= Weights[6];
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(0, 0, Weights[6]);
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);

        return (targetInWorldSpace - transform.position).normalized;
    }

    private Vector2 Cohesion(List<Vector3> friendPositions)
    {
        Vector3 result = new Vector3();

        if (friendPositions.Count > 0)
        {
            int agentCount = 0;

            for (int i = 0; i < friendPositions.Count; ++i)
            {
                result += friendPositions[i];
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

    private Vector2 Separation(List<Vector3> enemyPositions, List<Vector3> friendPositions)
    {
        Vector2 result = new Vector3();

        List<Vector3> positions;

        if (isPredator)
        {
            positions = enemyPositions;

            if (positions.Count == 0)
            {
                return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }
        else
        {
            positions = friendPositions;
        }

        for (int i = 0; i < positions.Count; ++i)
        {
            Vector3 towardsMe = transform.position - positions[i];

            if (towardsMe.magnitude > 0)
            {
                result = towardsMe.normalized / towardsMe.magnitude;
            }

            result.Normalize();
        }

        return result;
    }

    private Vector2 Allignment(List<Vector2> friendVelocities)
    {
        Vector2 result = new Vector2();

        if (friendVelocities.Count > 0)
        {
            for (int i = 0; i < friendVelocities.Count; ++i)
            {
                result += friendVelocities[i];
            }

            result.Normalize();
        }

        return result;
    }

    private Vector2 Flee(Vector3 targ)
    {
        Vector2 desiredVel = (transform.position - targ).normalized * maxVelocity;
        return desiredVel - Velocity;
    }

    public Vector2 Avoid(List<Vector3> enemyPositions)
    {
        Vector2 result = new Vector3();

        for (int i = 0; i < enemyPositions.Count; ++i)
        {
            result += Flee(enemyPositions[i]);
        }

        return result;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SearchRadius);
    }
#endif
}
