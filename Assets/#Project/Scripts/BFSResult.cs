using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct BFSResult 
{
   private Dictionary<Vector3Int, Vector3Int?> range;

    public BFSResult(Dictionary<Vector3Int, Vector3Int?> range) {
        this.range = range;
    }

    public bool isHexPositionInRange(Vector3Int position) {
        return range.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetHexInRange() {
        return range.Keys;
    }

    public List<Vector3Int> GetPathTo(Vector3Int position) {
        List<Vector3Int> result = new List<Vector3Int>();
        if (range.ContainsKey(position)) {
            for(Vector3Int? i = position; i != null; i = range[i.Value]) {
                result.Add(i.Value);
            }
            result.Reverse();
            result = result.Skip(1).ToList();
        }
        return result;
    }
}
