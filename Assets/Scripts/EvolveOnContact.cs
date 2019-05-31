using UnityEngine;
using System;

public class EvolveOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.tag.Equals(Tag.Predator))
        {
            bool playerInvolved = col.gameObject.tag.Equals(Tag.Player, StringComparison.Ordinal) ||
                                  gameObject.tag.Equals(Tag.Player, StringComparison.Ordinal);

            EvolutionManager.RegisterContact(name, col.name, playerInvolved);
        }
    }
}
