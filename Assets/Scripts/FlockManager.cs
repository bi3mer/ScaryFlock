using System.Collections.Generic;
using UnityEngine;

public class FlockManager : Singleton<FlockManager>
{
    private int key = int.MinValue;

    public Dictionary<string, FlockingAgent> flock { get; private set; }

    public int FlockCount { get; private set; }

    private void Awake()
    {
        FlockCount = 0;
        flock = new Dictionary<string, FlockingAgent>();
    }

    public void Reset()
    {
        foreach (KeyValuePair<string, FlockingAgent> kvp in flock)
        {
            Destroy(kvp.Value.gameObject);
        }

        flock.Clear();
        key = int.MinValue;
        FlockCount = 0;
    }

    public void AddAgent(FlockingAgent agent)
    {
        agent.name = key.ToString();

        flock.Add(agent.name, agent);
        ++key;
        ++FlockCount;
    }

    public void RemoveAgent(string id)
    {
        flock.Remove(id);
        --FlockCount;
    }

    public FlockingAgent Get(string id)
    {
        if (flock.ContainsKey(id))
        {
            return flock[id];
        }

        return null;
    }

    public List<FlockingAgent> GetAgentsInRadius(string id, float radius)
    {
        List<FlockingAgent> nearbyAgents = new List<FlockingAgent>();
        Vector2 position = flock[id].transform.position;

        foreach (KeyValuePair<string, FlockingAgent> kvp in flock)
        {
            if (kvp.Key.Equals(id))
            {
                continue;
            }

            FlockingAgent agent = kvp.Value;
            if (Vector2.Distance(position, agent.transform.position) < radius)
            {
                nearbyAgents.Add(agent);
            }
        }

        return nearbyAgents;
    }
}
