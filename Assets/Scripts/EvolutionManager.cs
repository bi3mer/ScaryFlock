using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine;

// TODO: instead of this just prevent two parents from ever mating together again and live a better life
public static class EvolutionManager
{
    private static readonly HashSet<string> touchedIds = new HashSet<string>();

    public static void Reset()
    {
        touchedIds.Clear();
    }

    public static void RegisterContact(string baseId, string colId, bool playerInvolved)
    {
        if (touchedIds.Contains(baseId)) return;
        if (touchedIds.Contains(colId)) return;

        touchedIds.Add(baseId);
        touchedIds.Add(colId);


        if(FlockManager.Instance.FlockCount < GameManager.Instance.MaxSpawnCount)
        {
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
        FlockingAgent agent1 = FlockManager.Instance.Get(id1);
        FlockingAgent agent2 = FlockManager.Instance.Get(id2);
        FlockingAgent newAgent = GameManager.Instance.CreateNewPrey();
        Evolution.Run(newAgent, (OnUpdateFlockingAgent)agent1, (OnUpdateFlockingAgent)agent2);
        newAgent.transform.position = new Vector3(
            agent1.transform.position.x + UnityEngine.Random.Range(-1f, 1f),
            agent1.transform.position.y + UnityEngine.Random.Range(-1f, 1f),
            agent1.transform.position.z);

        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(agent1.name));
        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(agent2.name));
    }

    private static void CreateNewPreyFromPlayer(string id1, string id2)
    {
        FlockingAgent agent1 = FlockManager.Instance.Get(id1);
        FlockingAgent agent2 = FlockManager.Instance.Get(id2);
        FlockingAgent parentPrey;

        if (agent1.tag.Equals(Tag.Prey, StringComparison.Ordinal))
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
        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(parentPrey.name));
    }

    private static IEnumerator FreeAgentToMateAgain(string id)
    {
        yield return new WaitForSeconds(5f);
        touchedIds.Remove(id);
    }
}
