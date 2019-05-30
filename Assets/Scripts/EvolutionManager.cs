using System.Collections.Generic;

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
        UnityEngine.Debug.Log("Creating prey form prey");
    }

    private static void CreateNewPreyFromPlayer(string id1, string id2)
    {
        UnityEngine.Debug.Log("Creating prey from player");
    }
}
