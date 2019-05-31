using System.Collections.Generic;

public class FlockManager : Singleton<FlockManager>
{
    private Dictionary<string, FlockingAgent> flock = new Dictionary<string, FlockingAgent>();
    private int key = int.MinValue;

    public int FlockCount { get; private set; }

    private void Awake()
    {
        FlockCount = 0;
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
}
