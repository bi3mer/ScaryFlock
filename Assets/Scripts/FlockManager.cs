using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class FlockManager : Singleton<FlockManager>
{
    private Dictionary<string, FlockingAgent> flock = new Dictionary<string, FlockingAgent>();
    private int key = int.MinValue;

    public int FlockCount { get; private set; }

    [SerializeField]
    private Score scoreUI;

    private void Awake()
    {
        Assert.IsNotNull(scoreUI);
        FlockCount = 0;
    }

    public void Reset()
    {
        foreach (KeyValuePair<string, FlockingAgent> kvp in flock)
        {
            Destroy(kvp.Value);
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

        scoreUI.UpdateScore(FlockCount);
    }

    public void RemoveAgent(string id)
    {
        flock.Remove(id);
        --FlockCount;
        scoreUI.UpdateScore(FlockCount);
    }

    public FlockingAgent Get(string id)
    {
        if (flock.ContainsKey(id))
        {
            return flock[id];
        }

        return null;
    }
}
