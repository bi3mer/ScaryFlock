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

    [SerializeField]
    [Range(10, 1500)]
    private int maxSpawned = 800;

    [SerializeField]
    [Range(10, 60)]
    private int secondsTillPredatorSpawns = 30;

    [SerializeField]
    private bool isMainMenu = false;

    private float time;

    public int Score { get; private set; }
    public int MaxSpawnCount => maxSpawned;
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

    private void Update()
    {
        if(!isMainMenu)
        {

            time += Time.deltaTime;

            if (time > secondsTillPredatorSpawns)
            {
                time = 0f;
                SpawnNewPredator();
            }
        }
    }

    public FlockingAgent CreateNewPrey()
    {
        FlockingAgent agent = Instantiate(prey);
        agent.transform.position = RandomInRectWorldPosition();
        FlockManager.Instance.AddAgent(agent);

        return agent;
    }

    public FlockingAgent SpawnNewPredator()
    {
        FlockingAgent agent = Instantiate(predator);

        float random = Random.Range(0f, 1f);
        if (random < 0.25f)
        {
            agent.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        }
        else if (random < 0.5f)
        {
            agent.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0f));
        }
        else if (random < 0.75f)
        {
            agent.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f));
        }
        else
        {
            agent.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        }

        agent.transform.position = new Vector3(agent.transform.position.x, agent.transform.position.y, 0);
        FlockManager.Instance.AddAgent(agent);

        return agent;
    }

    public void RestartGame()
    {
        // reset the scorevariables keeping track of past events
        time = 0f;
        Score = 0;

        if (!isMainMenu)
        {
            FlockManager.Instance.Reset();
            EvolutionManager.Reset();
        }

        // Spawn new agents into the world
        FlockingAgent tempInstantiator;
        int i;
        for (i = 0; i < startingPreyCount; ++i)
        {
            tempInstantiator = Instantiate(prey);
            tempInstantiator.transform.position = RandomInRectWorldPosition();
            tempInstantiator.RandomizeWeights();

            if (isMainMenu)
            {
                tempInstantiator.GetComponent<CircleCollider2D>().enabled = false;
            }
            else
            {
                FlockManager.Instance.AddAgent(tempInstantiator);
            }
        }

        if(!isMainMenu)
        {
            for (i = 0; i < startingPredatorCount; ++i)
            {
                tempInstantiator = Instantiate(predator);
                tempInstantiator.transform.position = RandomInRectWorldPosition();

                FlockManager.Instance.AddAgent(tempInstantiator);
            }

            tempInstantiator = Instantiate(player);
            Vector3 tempVector = Camera.main.ViewportToWorldPoint(new Vector2(0.1f, 0.1f));
            tempInstantiator.transform.position = new Vector3(tempVector.x, tempVector.y, 0f);
            
            FlockManager.Instance.AddAgent(tempInstantiator);
        }
    }
}
