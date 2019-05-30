using UnityEngine.Assertions;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField]
    //private Camera camera = null;

    [SerializeField]
    private GameObject prey = null;

    [SerializeField]
    private GameObject predator = null;

    [SerializeField]
    private GameObject player = null;

    [SerializeField]
    [Range(0, 500)]
    private int startingPreyCount = 50;

    [SerializeField]
    [Range(0, 20)]
    private int startingPredatorCount = 3;

    [SerializeField]
    private bool startOnStart = false;

    public int Score { get; private set; }

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
        if (startOnStart)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        // kill past children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // reset the scorevariables keeping track of past events
        Score = 0;

        // Spawn new agents into the world
        GameObject tempInstantiator;
        int i;
        for (i = 0; i < startingPreyCount; ++i)
        {
            tempInstantiator = Instantiate(prey);
            tempInstantiator.transform.position = RandomInRectWorldPosition();
            tempInstantiator.transform.SetParent(transform);
        }

        for(i = 0; i < startingPredatorCount; ++i)
        {
            tempInstantiator = Instantiate(predator);
            tempInstantiator.transform.SetParent(transform);
            tempInstantiator.transform.position = RandomInRectWorldPosition();
        }

        tempInstantiator = Instantiate(player);
        Vector3 tempVector = Camera.main.ViewportToWorldPoint(new Vector2(0.1f, 0.1f));
        tempInstantiator.transform.position = new Vector3(tempVector.x, tempVector.y, 0f);
        tempInstantiator.transform.SetParent(transform);
    }

}
