using UnityEngine;
using System;

public class EvolveOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.tag.Equals(Tag.Predator))
        {
            EvolutionManager.RegisterContact(name, col.name, col.gameObject.tag.Equals(Tag.Player, StringComparison.Ordinal));
        }
    }
}
