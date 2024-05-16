using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    List<Unit> pcs = new List<Unit>();
    List<Unit> exhausted = new List<Unit>();
    List<AbstarctNpc> npcs = new List<AbstarctNpc>();

    void Start() {
        foreach (GameObject pc in GameObject.FindGameObjectsWithTag("Player")) {
            pcs.Add(pc.GetComponent<Unit>());
        }
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC")) {
            npcs.Add(npc.GetComponent<AbstarctNpc>());
        }
    }

    public bool isAvailable(Unit unit) {
        return pcs.Contains(unit);
    }

    private void Exhaust(Unit unit) {
        if (pcs.Contains(unit)) {
            pcs.Remove(unit);
            exhausted.Add(unit);
        }
        if(pcs.Count == 0) {
            EndTurn();
            pcs = exhausted;
            exhausted = new List<Unit>();
        }
    }

    private void EndTurn() {
        foreach (AbstarctNpc npc in npcs)
        {
            npc.TakeTurn();
        }
    }
}
