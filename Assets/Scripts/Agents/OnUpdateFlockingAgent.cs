using UnityEngine;

public class OnUpdateFlockingAgent : FlockingAgent
{
    [SerializeField]
    private int maxMates = 10;

    private int matedCount = 0;
    public int MatedCount { get; private set; }

    public void AgentMated()
    {
        ++matedCount;

        if (matedCount >= maxMates)
        {
            FlockManager.Instance.RemoveAgent(name);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        OnUpdate();
    }
}
