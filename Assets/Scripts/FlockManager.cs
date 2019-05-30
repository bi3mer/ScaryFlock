using System.Collections.Generic;
using UnityEngine.Assertions;

public class FlockManager : Singleton<FlockManager>
{
    private Dictionary<string, FlockingAgent> flock = new Dictionary<string, FlockingAgent>();
    private int key = int.MinValue;

    public void Reset()
    {
        foreach(KeyValuePair<string, FlockingAgent> kvp in flock)
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
    }

    public void RemoveAgent(string id)
    {
        flock.Remove(id);
    }

    public FlockingAgent Get(string id)
    {
        Assert.IsTrue(flock.ContainsKey(id));
        return flock[id];
    }
}
