using UnityEngine.Assertions;
using UnityEngine;

public class PerformanceTester : MonoBehaviour
{
    [SerializeField]
    private FlockingAgent prey = null;

    [SerializeField]
    private int increaseFlockByCount = 25;

    public int FlockSize { get; private set; }

    private void Awake()
    {
        Assert.IsNotNull(prey);
        FlockSize = 0;
    }

    private void Start()
    {
        FlockingAgent.WorldMin = Camera.main.ViewportToWorldPoint(new Vector2(0.02f, 0.02f));
        FlockingAgent.WorldMax = Camera.main.ViewportToWorldPoint(new Vector2(0.98f, 0.98f));
    }

    public void IncreaseFlockSize()
    {
        for (int i = 0; i < increaseFlockByCount; ++i)
        {
            FlockingAgent agent = Instantiate(prey);
            agent.transform.position = GameManager.RandomInRectWorldPosition();
            agent.RandomizeWeights();
            agent.GetComponent<CircleCollider2D>().enabled = false;

            FlockManager.Instance.AddAgent(agent);
        }

        FlockSize += increaseFlockByCount;
    }
}
