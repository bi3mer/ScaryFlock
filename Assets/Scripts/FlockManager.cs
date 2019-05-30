using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class FlockManager : Singleton<FlockManager>
{
    private Dictionary<string, FlockingAgent> flock = new Dictionary<string, FlockingAgent>();
    private int key = int.MinValue;

    [SerializeField]
    private Score scoreUI;

    private void Awake()
    {
        Assert.IsNotNull(scoreUI);
    }

    public void Reset()
    {
        foreach (KeyValuePair<string, FlockingAgent> kvp in flock)
        {
            Destroy(kvp.Value);
        }

        flock.Clear();
        key = int.MinValue;
    }

    public void AddAgent(FlockingAgent agent)
    {
        agent.name = key.ToString();

        flock.Add(agent.name, agent);
        ++key;

        scoreUI.UpdateScore();
    }

    public void RemoveAgent(string id)
    {
        flock.Remove(id);
        scoreUI.UpdateScore();
    }

    public FlockingAgent Get(string id)
    {
        Assert.IsTrue(flock.ContainsKey(id));
        return flock[id];
    }
}
