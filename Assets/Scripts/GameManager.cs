using UnityEngine.Assertions;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField]
    //private Camera camera = null;

    [SerializeField]
    private FlockingAgent prey = null;

    [SerializeField]
    private FlockingAgent predator = null;

    [SerializeField]
    private FlockingAgent player = null;

    [SerializeField]
    [Range(0, 500)]
    private int startingPreyCount = 50;

    [SerializeField]
    [Range(0, 20)]
    private int startingPredatorCount = 3;

    [SerializeField]
    private bool startOnStart = false;

    public int PreyCount { get; private set; }
    public int Score { get; private set; }
    public FlockingAgent Prey => prey;

    private Vector3 RandomInRectWorldPosition()
    {
        Vector3 tempPosition = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.2f, 1f), Random.Range(0.2f, 1f)));
        return new Vector3(tempPosition.x, tempPosition.y, 0f);
    }

    private void Awake()
    {
        Assert.IsNotNull(prey);
        Assert.IsNotNull(predator);
        Assert.IsNotNull(player);
    }

    private void Start()
    {
        FlockingAgent.WorldMin = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        FlockingAgent.WorldMax = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        FlockingAgent.PredatorMask = LayerMask.NameToLayer(Tag.Predator);
        FlockingAgent.FriendlyMask = LayerMask.GetMask(Tag.Prey, Tag.Player);

        if (startOnStart)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        // reset the scorevariables keeping track of past events
        Score = 0;
        PreyCount = 0;
        FlockManager.Instance.Reset();
        EvolutionManager.Reset();

        // Spawn new agents into the world
        FlockingAgent tempInstantiator;
        int i;
        for (i = 0; i < startingPreyCount; ++i)
        {
            tempInstantiator = Instantiate(prey);
            tempInstantiator.transform.position = RandomInRectWorldPosition();
            tempInstantiator.RandomizeWeights();
            ++PreyCount;
            
            FlockManager.Instance.AddAgent(tempInstantiator);

        }

        for (i = 0; i < startingPredatorCount; ++i)
        {
            tempInstantiator = Instantiate(predator);
            tempInstantiator.transform.position = RandomInRectWorldPosition();

            FlockManager.Instance.AddAgent(tempInstantiator);
        }

        tempInstantiator = Instantiate(player);
        Vector3 tempVector = Camera.main.ViewportToWorldPoint(new Vector2(0.1f, 0.1f));
        tempInstantiator.transform.position = new Vector3(tempVector.x, tempVector.y, 0f);
        tempInstantiator.RandomizeWeights();

        FlockManager.Instance.AddAgent(tempInstantiator);
    }
}
