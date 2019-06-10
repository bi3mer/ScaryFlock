using System.Collections.Generic;
using UnityEngine;

public class FlockManager : Singleton<FlockManager>
{
    private int key = int.MinValue;

    public Dictionary<string, BOIDAgent> Flock { get; private set; }

    public int FlockCount { get; private set; }

    private void Awake()
    {
        FlockCount = 0;
        Flock = new Dictionary<string, BOIDAgent>();
    }

    public void Reset()
    {
        Flock.Clear();
        key = int.MinValue;
        FlockCount = 0;
    }

    public void AddAgent(BOIDAgent agent)
    {
        Flock.Add(key.ToString(), agent);
        ++key;
        ++FlockCount;
    }

    public void RemoveAgent(string id)
    {
        Flock.Remove(id);
        --FlockCount;
    }

    public BOIDAgent Get(string id) => Flock[id];

    public List<BOIDAgent> GetAgentsInRadius(Vector3 position, float radius)
    {
        List<BOIDAgent> nearbyAgents = new List<BOIDAgent>();

        foreach (KeyValuePair<string, BOIDAgent> kvp in Flock)
        {
            BOIDAgent agent = kvp.Value;
            if (Vector2.Distance(position, agent.Position) < radius)
            {
                nearbyAgents.Add(agent);
            }
        }

        return nearbyAgents;
    }
}
