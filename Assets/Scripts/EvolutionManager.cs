using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine;

// TODO: instead of this just prevent two parents from ever mating together again and live a better life
public static class EvolutionManager
{
    private static readonly HashSet<string> touchedIds = new HashSet<string>();
    private const float playerMateCoolDown = 0.001f;
    private const float preyMateCoolDoown = 5f;

    public static void Reset()
    {
        touchedIds.Clear();
        GameManager.Instance.StopAllCoroutines();
    }

    public static void RegisterContact(string baseId, string colId, bool playerInvolved)
    {
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
        if (touchedIds.Contains(id1)) return;
        if (touchedIds.Contains(id2)) return;

        touchedIds.Add(id1);
        touchedIds.Add(id2);

        FlockingAgent agent1 = FlockManager.Instance.Get(id1);
        FlockingAgent agent2 = FlockManager.Instance.Get(id2);

        if (agent1 == null || agent2 == null)
        {
            return;
        }

        FlockingAgent newAgent = GameManager.Instance.CreateNewPrey();
        Evolution.Run(newAgent, (OnUpdateFlockingAgent)agent1, (OnUpdateFlockingAgent)agent2);
        newAgent.transform.position = new Vector3(
            agent1.transform.position.x + UnityEngine.Random.Range(-1f, 1f),
            agent1.transform.position.y + UnityEngine.Random.Range(-1f, 1f),
            agent1.transform.position.z);

        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(agent1.name, preyMateCoolDoown));
        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(agent2.name, preyMateCoolDoown));
    }

    private static void CreateNewPreyFromPlayer(string id1, string id2)
    {
        FlockingAgent agent1 = FlockManager.Instance.Get(id1);
        FlockingAgent agent2 = FlockManager.Instance.Get(id2);

        if (agent1 == null || agent2 == null)
        {
            return;
        }

        FlockingAgent parentPrey;
        FlockingAgent player;

        if (agent1.tag.Equals(Tag.Prey, StringComparison.Ordinal))
        {
            parentPrey = agent1;
            player = agent2;
        }
        else
        {
            parentPrey = agent2;
            player = agent1;
        }

        if (touchedIds.Contains(parentPrey.name)) return;
        if (touchedIds.Contains(player.name)) return;

        ((OnUpdateFlockingAgent)parentPrey).AgentMated();
        FlockingAgent newAgent = UnityEngine.Object.Instantiate(parentPrey);
        FlockManager.Instance.AddAgent(newAgent);

        touchedIds.Add(player.name);
        touchedIds.Add(newAgent.name);
        touchedIds.Add(parentPrey.name);

        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(player.name, playerMateCoolDown));
        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(newAgent.name, playerMateCoolDown));
        GameManager.Instance.StartCoroutine(FreeAgentToMateAgain(parentPrey.name, preyMateCoolDoown));
    }

    private static IEnumerator FreeAgentToMateAgain(string id, float time)
    {
        yield return new WaitForSeconds(time);
        touchedIds.Remove(id);
    }
}
