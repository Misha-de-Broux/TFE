using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomNpc : AbstarctNpc {

    protected override void Start() {
        base.Start();
    }

    public override void TakeTurn() {
        BFSResult range = GraphSearch.BFSGetRange(grid, unit.HexCoord, unit.mouvement);
        List<Vector3Int> hexInRange = new List<Vector3Int>(range.GetHexInRange());
        Vector3Int destination = hexInRange[Random.Range(0, hexInRange.Count)];
        List<Vector3Int> path = new List<Vector3Int>();
        if (destination != unit.HexCoord) {
            path = range.GetPathTo(destination);
            
        } else {
            path.Add(destination);
        }
        unit.MouvementFinished += (Unit unit) => OnEndMouvement();
        unit.Move(path.Select(pos => grid[pos].transform.position).ToList());
    }


    private void OnEndMouvement() {
        unit.MouvementFinished -= (Unit unit) => OnEndMouvement();
        OnEndTurn();
    }
}
