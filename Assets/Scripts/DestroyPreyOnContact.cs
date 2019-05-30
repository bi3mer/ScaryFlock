using UnityEngine.Assertions;
using UnityEngine;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class DestroyPreyOnContact : MonoBehaviour
{
    private CircleCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        Assert.IsNotNull(collider);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("player", StringComparison.Ordinal))
        {
            Debug.Log("Game Over");
        }
        else if (col.gameObject.tag.Equals("prey", StringComparison.Ordinal))
        {
            FlockManager.Instance.RemoveAgent(col.gameObject.name);
            Destroy(col.gameObject);
        }
    }
}
