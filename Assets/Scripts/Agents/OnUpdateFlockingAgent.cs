using UnityEngine;

public class OnUpdateFlockingAgent : FlockingAgent
{
    [SerializeField]
    private int maxMates = 3;

    private int matedCount = 0;
    public int MatedCount => matedCount;

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
