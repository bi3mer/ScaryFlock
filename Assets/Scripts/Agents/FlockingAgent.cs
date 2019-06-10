using System.Collections.Generic;
using UnityEngine;

public struct BOIDAgent
{
    public static readonly Vector2 WorldMin;
    public static readonly Vector2 WorldMax;

    public readonly float MaxAcceleration;
    public readonly float MaxVelocity;

    public readonly float SearchRadius;

    public readonly float CohesionWeight;
    public readonly float SeparationWeight;
    public readonly float AllignmentWeight;
    public readonly float WanderWeight;
    public readonly float AvoidWeight;
    public readonly float Jitter;

    public readonly bool IsPredator;

    private int timesMated;

    public Vector3 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public Vector2 Acceleration { get; private set; }
    public Vector3 WanderTarget { get; private set; }

    private float RandomBinomial => Random.Range(0f, 1f) - Random.Range(0f, 1f);

    public BOIDAgent Copy => new BOIDAgent(
        SearchRadius,
        CohesionWeight,
        SeparationWeight,
        AllignmentWeight,
        WanderWeight,
        AvoidWeight,
        Jitter,
        MaxAcceleration,
        MaxVelocity,
        Position,
        IsPredator,
        timesMated);

    public BOIDAgent(
        float maxRadius, 
        float maxWeight, 
        float maxAcceleration, 
        float maxVelocity,
        Vector3 position)
    {
        SearchRadius = Random.Range(0f, maxRadius);

        CohesionWeight = Random.Range(0f, maxWeight);
        SeparationWeight = Random.Range(0f, maxWeight);
        AllignmentWeight = Random.Range(0f, maxWeight);
        WanderWeight = Random.Range(0f, maxWeight);
        AvoidWeight = Random.Range(0f, maxWeight);
        Jitter = Random.Range(0f, maxWeight);

        Acceleration = Vector2.zero;
        WanderTarget = Vector3.zero;
        Velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        MaxAcceleration = maxAcceleration;
        MaxVelocity = maxVelocity;

        Position = position;
        IsPredator = false;
        timesMated = 0;
    }

    public BOIDAgent(
        float searchRadius,
        float cohesionWeight,
        float separationWeight,
        float allignmentWeight,
        float wanderWeight,
        float avoidWeight,
        float jitter, 
        float maxAcceleration,
        float maxVelocity,
        Vector3 position, 
        bool isPredator,
        int timesMated)
    {
        SearchRadius = searchRadius;
        SeparationWeight = separationWeight;
        CohesionWeight = cohesionWeight;
        AllignmentWeight = allignmentWeight;
        WanderWeight = wanderWeight;
        AvoidWeight = avoidWeight;
        Jitter = jitter;

        Acceleration = Vector2.zero;
        WanderTarget = Vector3.zero;
        Velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        MaxAcceleration = maxAcceleration;
        MaxVelocity = maxVelocity;

        Position = position;
        IsPredator = isPredator;
        this.timesMated = timesMated;
    }

    public void Update()
    {
        // optimzation problems so lets stop the agents from being able to update every frame and 
        // do a lot of work every frame
        List<BOIDAgent> neighbors = FlockManager.Instance.GetAgentsInRadius(Position, SearchRadius);

        List<Vector2> friendVelocities = new List<Vector2>(); 
        List<Vector3> friendPositions = new List<Vector3>();
        List<Vector3> enemyPositions = new List<Vector3>();

        BOIDAgent neighbor;
        for (int i = 0; i < neighbors.Count; ++i)
        {
            neighbor = neighbors[i];

            if (neighbor.IsPredator)
            {
                enemyPositions.Add(neighbor.Position);
            }
            else
            {
                friendPositions.Add(neighbor.Position);
                friendVelocities.Add(neighbor.Velocity);
            }
        }

        Vector2 temp = Acceleration + Combine(enemyPositions, friendPositions, friendVelocities);
        Acceleration = Vector2.ClampMagnitude(temp, MaxAcceleration);
        Velocity = Vector2.ClampMagnitude(Velocity + Acceleration * Time.deltaTime, MaxVelocity);

        Position = Position + (Vector3)(Velocity * Time.deltaTime);
        KeepInBounds();
    }

    private void KeepInBounds()
    {
        float x = Position.x;
        float y = Position.y;
        float z = Position.z;

        if (x < WorldMin.x)
        {
            Position = new Vector3(WorldMin.x, y, z);
            Acceleration = new Vector3(0.5f, Acceleration.y);
        }

        if (x > WorldMax.x)
        {
            Position = new Vector3(WorldMax.x, y, z);
            Acceleration = new Vector3(-0.5f, Acceleration.y);
        }

        if (y < WorldMin.y)
        {
            Position = new Vector3(Position.x, WorldMin.y, z);
            Acceleration = new Vector3(Acceleration.x, 0.5f);
        }

        if (y > WorldMax.y)
        {
            Position = new Vector3(Position.x, WorldMax.y, z);
            Acceleration = new Vector3(Acceleration.x, -0.5f);
        }
    }

    private Vector2 Combine(List<Vector3> enemyPositions, List<Vector3> friendPositions, List<Vector2> friendVelocities)
    {
        if (IsPredator)
        {
            return CohesionWeight * Cohesion(friendPositions) +
                SeparationWeight  * Separation(enemyPositions, friendPositions) +
                AllignmentWeight  * Allignment(friendVelocities);
        }
        else
        {
            return CohesionWeight * Cohesion(friendPositions) +
                SeparationWeight  * Separation(enemyPositions, friendPositions) +
                AllignmentWeight  * Allignment(friendVelocities) +
                AvoidWeight       * Avoid(enemyPositions) +
                WanderWeight      * Wander();
        }
    }

    private Vector2 Wander()
    {
        float jitter = Jitter * Time.deltaTime;
        WanderTarget += new Vector3(RandomBinomial * jitter, RandomBinomial * jitter, 0);
        WanderTarget *= SearchRadius;
        WanderTarget.Normalize();

        return (WanderTarget + new Vector3(0, 0, SearchRadius) - Position).normalized;
    }

    private Vector2 Cohesion(List<Vector3> friendPositions)
    {
        Vector3 cohesion = new Vector3();

        if (friendPositions.Count > 0)
        {
            int agentCount = 0;

            for (int i = 0; i < friendPositions.Count; ++i)
            {
                cohesion += friendPositions[i];
                ++agentCount;
            }

            if (agentCount > 0)
            {
                cohesion /= agentCount;
            }

            cohesion = cohesion - Position;
            cohesion.Normalize();
        }

        return cohesion;
    }

    private Vector2 Separation(List<Vector3> enemyPositions, List<Vector3> friendPositions)
    {
        Vector2 separation = new Vector3();

        List<Vector3> positions;

        if (IsPredator)
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
            Vector3 towardsMe = Position - positions[i];

            if (towardsMe.magnitude > 0)
            {
                separation = towardsMe.normalized / towardsMe.magnitude;
            }

            separation.Normalize();
        }

        return separation;
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
        Vector2 desiredVel = (Position - targ).normalized * MaxVelocity;
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
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class FlockingAgent : MonoBehaviour
{
    public void OnUpdate()
    {
        
    }

    // @todo: why am I not using wander?
  
}
