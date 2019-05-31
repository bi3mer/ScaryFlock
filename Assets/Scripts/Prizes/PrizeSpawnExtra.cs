using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PrizeSpawnExtra : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals(Tag.Player))
        {
            int amountToBeSpawned = Random.Range(50, 100);

            for (int i = 0; i < amountToBeSpawned; ++i)
            {
                FlockingAgent agent = GameManager.Instance.CreateNewPrey();
                agent.RandomizeWeights();
            }

            Destroy(gameObject);
        }
    }
}
