using UnityEngine;
using System;

public class DestroyPreyOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals(Tag.Player, StringComparison.Ordinal))
        {
            GameStateMachine.Instance.GotoNextState();
        }
        else if (col.gameObject.tag.Equals(Tag.Prey, StringComparison.Ordinal))
        {
            FlockManager.Instance.RemoveAgent(col.gameObject.name);
            Destroy(col.gameObject);
        }
    }
}
