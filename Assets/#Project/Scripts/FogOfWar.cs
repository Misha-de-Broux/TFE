using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour {

    private Dictionary<Vector2Int, List<Hex>> flatMap = new Dictionary<Vector2Int, List<Hex>>();
    private Dictionary<Unit, List<Hex>> eyes = new Dictionary<Unit, List<Hex>>();

    [SerializeField] LayerMask fogMask = new LayerMask();

    // Start is called before the first frame update
    void Start() {
        foreach (GameObject playableCharacter in GameObject.FindGameObjectsWithTag("Player")) {
            Unit eye = playableCharacter.GetComponent<Unit>();
            eye.onEndingStep += () => UpdateFog(eye);
            AddEye(eye);
        }
    }

    public void AddHex(Hex hex) {
        Vector2Int flatCoord = new Vector2Int(hex.HexCoords.x, hex.HexCoords.z);
        if (!flatMap.ContainsKey(flatCoord)) {
            flatMap[flatCoord] = new List<Hex>();
        }
        flatMap[flatCoord].Add(hex);
        foreach (Unit unit in eyes.Keys) {
            if (Sees(unit, hex)) {
                hex.See();
                eyes[unit].Add(hex);
            }
        }
    }

    public void RemoveHex(Hex hex) {
        Vector2Int flatCoord = new Vector2Int(hex.HexCoords.x, hex.HexCoords.z);
        flatMap[flatCoord].Remove(hex);
        foreach (Unit unit in eyes.Keys) {
            if (eyes[unit].Remove(hex)) {
                hex.UnSee();
            }
        }
    }

    public void AddEye(Unit eye) {
        List<Hex> seenHexes = new List<Hex>();
        eyes[eye] = seenHexes;
    }

    public void RemoveEye(Unit eye) {
        eyes.Remove(eye);
    }

    private List<Vector2Int> CloserThan(int distance) {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int i = -distance; i <= distance; i++) {
            for (int j = -distance; j <= distance; j++) {
                if (Mathf.Abs(i + j) <= distance) {
                    result.Add(new Vector2Int(i, j));
                }
            }
        }
        return result;
    }

    private void UpdateFog(Unit unit) {
        foreach (Hex hex in eyes[unit]) {
            hex.UnSee();
        }
        eyes[unit].Clear();
        Vector2Int unitFlatPos = new Vector2Int(unit.HexCoord.x, unit.HexCoord.z);
        foreach (Vector2Int direction in CloserThan(unit.sightDistance)) {
            foreach (Hex hex in HexesAt(unitFlatPos + direction)) {
                if (Sees(unit, hex)) {
                    eyes[unit].Add(hex);
                } else {
                    Debug.Log(hex.HexCoords);
                }
            }
        }
        foreach (Hex hex in eyes[unit]) {
            hex.See();
        }
    }

    private bool Sees(Unit unit, Hex hex) {
        if (Mathf.Abs((unit.HexCoord.x - hex.HexCoords.x) + (unit.HexCoord.z - hex.HexCoords.z)) > unit.sightDistance) {
            return false;
        }
        RaycastHit hit;
        Vector3 eyesPosition = unit.transform.position + unit.eyesHeight * Vector3.up;
        float seenColiderHeight = hex.IsVisible(true);
        int loopNumber = 2;
        for (int i = 0; i <= loopNumber; i++) {
            if (Physics.Raycast(eyesPosition, hex.transform.position + i * seenColiderHeight * Vector3.up / loopNumber - eyesPosition, out hit, float.MaxValue, fogMask)) {
                if (hit.collider.GetComponent<Hex>() == hex) {
                    hex.IsVisible(false);
                    return true;
                }
            }
        }
        hex.IsVisible(false);
        return false;
    }

    private List<Hex> HexesAt(Vector2Int position) {
        try {
            return flatMap[position] ?? new List<Hex>();
        } catch (KeyNotFoundException) {
            return new List<Hex>();
        }

    }


}
