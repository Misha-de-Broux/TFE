using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof (FogOfWar))]
public class HexGrid : MonoBehaviour {

    private static List<Vector3Int> _neigbours = new List<Vector3Int> {
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(0, 1, 1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, 1, -1),
        new Vector3Int(-1, 1, 1),
        new Vector3Int(0, -1, 1),
        new Vector3Int(0, -1, -1),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, -1, -1),
        new Vector3Int(-1, -1, 1),
        new Vector3Int(0, -2, 1),
        new Vector3Int(0, -2, -1),
        new Vector3Int(1, -2, 0),
        new Vector3Int(-1, -2, 0),
        new Vector3Int(1, -2, -1),
        new Vector3Int(-1, -2, 1),
        new Vector3Int(0, -3, 1),
        new Vector3Int(0, -3, -1),
        new Vector3Int(1, -3, 0),
        new Vector3Int(-1, -3, 0),
        new Vector3Int(1, -3, -1),
        new Vector3Int(-1, -3, 1)
    };

    private FogOfWar _fogOfWar;

    Dictionary<Vector3Int, Hex> tileGrid = new Dictionary<Vector3Int, Hex>();
    Dictionary<Vector3Int, List<Vector3Int>> neighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();

    private void Start() {
        _fogOfWar = GetComponent<FogOfWar>();
        foreach (Hex hex in FindObjectsOfType<Hex>()) {  
            hex.onCover += () => RemoveHex(hex);
            hex.onReveal += () => RevealHex(hex);
            RevealHex(hex);
        }
    }

    public Hex this[Vector3Int coords] {
        get {
            if (tileGrid.ContainsKey(coords)) {
                return tileGrid[coords];
            }
            return null;
        }
    }

    public List<Vector3Int> GetNeighboursFor(Vector3Int coord) {
        if (!tileGrid.ContainsKey(coord)) {
            return new List<Vector3Int>();
        }
        if (neighboursDict.ContainsKey(coord)) {
            return neighboursDict[coord];
        } else {
            neighboursDict.Add(coord, new List<Vector3Int>());
            foreach (Vector3Int direction in _neigbours) {
                if (tileGrid.ContainsKey(coord + direction)) {
                    neighboursDict[coord].Add(coord + direction);
                }
            }
            return neighboursDict[coord];
        }
    }

    private void RemoveHex(Hex hex) {
        if (tileGrid.Remove(hex.HexCoords)) {
            _fogOfWar.RemoveHex(hex);
        }
    }

    private void RevealHex(Hex hex) {
        try{
            tileGrid.Add(hex.HexCoords, hex);
            _fogOfWar.AddHex(hex);
        } catch(ArgumentException) { }
    }
}

