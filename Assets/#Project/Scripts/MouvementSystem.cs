using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouvementSystem : MonoBehaviour {
    private BFSResult _range = new BFSResult();
    private List<Vector3Int> _path = new List<Vector3Int>();

    public void HideRange(HexGrid grid) {
        foreach (Vector3Int hexPosition in _range.GetHexInRange()) {
            grid[hexPosition].DisableHighlight();
        }
        _range = new BFSResult();
    }

    public void showRange(Unit unit, HexGrid grid) {
        CalculateRange(unit, grid);
        foreach (Vector3Int hexPosition in _range.GetHexInRange()) {
            grid[hexPosition].EnnableHighlight();
        }
    }

    public void ShowPath(Vector3Int destination, HexGrid grid) {
        if (_range.GetHexInRange().Contains(destination)) {
            foreach (Vector3Int hexPosition in _path) {
                grid[hexPosition].ResetHighlight();
            }
            _path = _range.GetPathTo(destination);
            foreach (Vector3Int hexPosition in _path) {
                grid[hexPosition].HighLightPath();
            }
        }
    }

    public void MoveUnit(Unit unit, HexGrid grid) {
        unit.Move(_path.Select(pos => grid[pos].transform.position).ToList());
    }

    public bool IsHexInRange(Vector3Int hexPosition) {
        return _range.GetHexInRange().Contains(hexPosition);
    }

    private void CalculateRange(Unit unit, HexGrid grid) {
        _range = GraphSearch.BFSGetRange(grid, unit.HexCoord, unit.mouvement);
    }

}
