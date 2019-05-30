using UnityEngine;

public class OnUpdateFlockingAgent : FlockingAgent
{
    [SerializeField]
    private int lives = 3;

    public void AgentMated()
    {
        --lives;

        if (lives <= 0)
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
