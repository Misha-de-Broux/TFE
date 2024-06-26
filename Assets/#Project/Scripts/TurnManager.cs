using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnManager : MonoBehaviour
{
    List<Unit> pcs = new List<Unit>();
    List<Unit> exhaustedPcs = new List<Unit>();
    List<Unit> strandedPCs = new List<Unit>();
    List<AbstarctNpc> npcs = new List<AbstarctNpc>();
    List<AbstarctNpc> exhaustedNpcs = new List<AbstarctNpc>();
    

    
    void Start() {
        foreach (GameObject pc in GameObject.FindGameObjectsWithTag("Player")) {
            Unit unit = pc.GetComponent<Unit>();
            if (!pcs.Contains(unit)) {
                pcs.Add(unit);
                unit.MouvementFinished += Exhaust;
            }
        }
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC")) {
            AbstarctNpc anpc = npc.GetComponent<AbstarctNpc>();
            if (!npcs.Contains(anpc)) {
                npcs.Add(anpc);
                anpc.EndTurn += Exhaust;
            }
        }
    }

    private void Exhaust(AbstarctNpc npc) {
        if (npcs.Contains(npc)){
            npcs.Remove(npc);
            exhaustedNpcs.Add(npc);
        }
        NpcTurn();
    }

    public bool isAvailable(Unit unit) {
        return pcs.Contains(unit);
    }

    private void Exhaust(Unit unit) {
        if (pcs.Contains(unit)) {
            pcs.Remove(unit);
            exhaustedPcs.Add(unit);
        }
        foreach(Unit pc in pcs) {
            if (!pc.IsActionable()) {
                strandedPCs.Add(pc);
            }
        }
        List<Unit> newStranded = new List<Unit>();
        foreach(Unit stranded in strandedPCs) {
            if (stranded.IsActionable()) {
                pcs.Add(stranded);
            } else {
                if (pcs.Contains(stranded)) {
                    pcs.Remove(stranded);
                }
                newStranded.Add(stranded);
            }
        }
        strandedPCs = newStranded;
        if (pcs.Count == 0) {
            npcs.AddRange(exhaustedNpcs);
            exhaustedNpcs.Clear();
            NpcTurn();
        }
    }

    private void NpcTurn() {
        if (npcs.Count == 0) {
            foreach(Unit unit in exhaustedPcs) {
                if (unit.IsActionable()) {
                    pcs.Add(unit);
                } else {
                    strandedPCs.Add(unit);
                }
            }
            exhaustedPcs.Clear();
        } else {
            if (npcs[0] == null) {
                npcs.RemoveAt(0);
                NpcTurn();
            } else {
                npcs[0].TakeTurn();
            }
        }
    }
}
