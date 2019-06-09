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
    private FlockingAgent ultraPredator = null;

    [SerializeField]
    private GameObject prize = null;

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

    [SerializeField]
    private Score scoreUI;

    private float time;

    public int Score { get; private set; }
    public int PredatorCount { get; private set; }
    public Transform Player { get; private set; }
    public int MaxSpawnCount => maxSpawned;
    public FlockingAgent Prey => prey;
    
    public static Vector3 RandomInRectWorldPosition()
    {
        Vector3 tempPosition = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.2f, 0.95f), Random.Range(0.2f, 0.95f)));
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
        FlockingAgent.WorldMin = Camera.main.ViewportToWorldPoint(new Vector2(0.02f, 0.02f));
        FlockingAgent.WorldMax = Camera.main.ViewportToWorldPoint(new Vector2(0.98f, 0.98f));
        FlockingAgent.PredatorMask = LayerMask.NameToLayer(Tag.Predator);
        FlockingAgent.FriendlyMask = LayerMask.GetMask(Tag.Prey, Tag.Player);
        FlockingAgent.AllMask = LayerMask.GetMask(Tag.Prey, Tag.Player, Tag.Predator);

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

                if (PredatorCount % 2 ==0)
                {
                    FlockingAgent predator = Instantiate(ultraPredator);
                    predator.transform.position = FlockingAgent.WorldMin;
                    predator.GetComponent<FollowPlayer>().Speed = Random.Range(0.5f, 1.7f);
                    FlockManager.Instance.AddAgent(predator);
                    ++PredatorCount;
                }

                if (FlockManager.Instance.FlockCount < 100)
                {
                    GameObject go = Instantiate(prize);
                    go.transform.position = RandomInRectWorldPosition();
                }
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

    public Vector3 GetRandomCorner()
    {
        Vector3 newPosition;
        float random = Random.Range(0f, 1f);
        if (random < 0.25f)
        {
            newPosition = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        }
        else if (random < 0.5f)
        {
            newPosition = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0f));
        }
        else if (random < 0.75f)
        {
            newPosition = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f));
        }
        else
        {
            newPosition = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        }

        return new Vector3(newPosition.x, newPosition.y, 0);
    }

    public FlockingAgent SpawnNewPredator()
    {
        FlockingAgent agent = Instantiate(predator);
        agent.transform.position = GetRandomCorner();
        FlockManager.Instance.AddAgent(agent);
        ++PredatorCount;

        return agent;
    }

    public void Reset()
    {
        time = 0f;
        Score = 0;
        PredatorCount = startingPredatorCount;

        if (!isMainMenu)
        {
            FlockManager.Instance.Reset();
            EvolutionManager.Reset();
        }

    }

    public void RestartGame()
    {
        Reset();

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
            Player = tempInstantiator.transform;
            
            FlockManager.Instance.AddAgent(tempInstantiator);
        }
    }

    public void UpdateScore()
    {
        Score += PredatorCount;
        scoreUI.UpdateScore(Score);
    }

    public void HideScoreUI()
    {
        scoreUI.gameObject.SetActive(false);
    }

    public void ShowScoreUI()
    {
        scoreUI.gameObject.SetActive(true);
    }
}
