using System.Collections.Generic;
using System;

// TODO: instead of this just prevent two parents from ever mating together again and live a better life
public static class EvolutionManager
{
    private static HashSet<string> touchedIds = new HashSet<string>();

    public static void Reset()
    {
        touchedIds.Clear();
    }

    public static void RegisterContact(string baseId, string colId, bool playerInvolved)
    {
        bool canMate = true;

        if (touchedIds.Contains(baseId) == false)
        {
            touchedIds.Add(baseId);
            canMate = false;
        }

        if (touchedIds.Contains(colId))
        {
            touchedIds.Add(colId);
            canMate = false;
        }

        if (canMate)
        {
            touchedIds.Remove(baseId);
            touchedIds.Remove(colId);

            if (playerInvolved)
            {
                CreateNewPreyFromPlayer(baseId, colId);
            }
            else
            {
                CreateNewPreyFromPrey(baseId, colId);
            }
        }
    }

    private static void CreateNewPreyFromPrey(string id1, string id2)
    {
        Evolution.Run(
            GameManager.Instance.Prey,
            FlockManager.Instance.Get(id1),
            FlockManager.Instance.Get(id2));
    }

    private static void CreateNewPreyFromPlayer(string id1, string id2)
    {
        FlockingAgent agent1 = FlockManager.Instance.Get(id1);
        FlockingAgent agent2 = FlockManager.Instance.Get(id2);
        FlockingAgent parentPrey;

        if (agent1.name.Equals(Tag.Prey, StringComparison.Ordinal))
        {
            parentPrey = agent1;
        }
        else
        {
            parentPrey = agent2;
        }

        ((OnUpdateFlockingAgent)parentPrey).AgentMated();
        FlockingAgent newAgent = UnityEngine.Object.Instantiate(parentPrey);
        FlockManager.Instance.AddAgent(newAgent);
    }
}
