using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour {

    private Dictionary<Vector2Int, List<Hex>> flatMap = new Dictionary<Vector2Int, List<Hex>>();
    private List<Unit> eyes = new List<Unit>();

    [SerializeField] LayerMask fogMask = new LayerMask();

    // Start is called before the first frame update
    void Start() {
        foreach (GameObject playableCharacter in GameObject.FindGameObjectsWithTag("Player")) {
            Unit eye = playableCharacter.GetComponent<Unit>();
            AddEye(eye);
        }
    }

    public void AddHex(Hex hex) {
        Vector2Int flatCoord = new Vector2Int(hex.HexCoords.x, hex.HexCoords.z);
        if (!flatMap.ContainsKey(flatCoord)) {
            flatMap[flatCoord] = new List<Hex>();
        }
        flatMap[flatCoord].Add(hex);
        foreach (Unit unit in eyes) {
            if (Sees(unit, hex)) {
                hex.See();
                unit.hexesSeen.Add(hex);
            }
        }
    }

    public void UpdateFogOfwar() {
        foreach(Unit unit in eyes) {
            UpdateFog(unit);
        }
    }

    public void RemoveHex(Hex hex) {
        Vector2Int flatCoord = new Vector2Int(hex.HexCoords.x, hex.HexCoords.z);
        flatMap[flatCoord].Remove(hex);
        foreach (Unit unit in eyes) {
            if (unit.hexesSeen.Remove(hex)) {
                hex.UnSee();
            }
        }
    }

    public void AddEye(Unit eye) {
        eyes.Add(eye);
        eye.onEndingStep += (Unit unit) => UpdateFog(unit);
        UpdateFog(eye);
    }

    public void RemoveEye(Unit eye) {
        foreach(Hex hex in eye.hexesSeen) {
            hex.UnSee();
        }
        eye.onEndingStep -= (Unit unit) => UpdateFog(unit);
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
        foreach (Hex hex in unit.hexesSeen) {
            hex.UnSee();
        }
        UpdateSeen(unit);
        foreach (Hex hex in unit.hexesSeen) {
            hex.See();
        }
    }

    private void UpdateSeen(Unit unit) {
        unit.hexesSeen.Clear();
        Vector2Int unitFlatPos = new Vector2Int(unit.HexCoord.x, unit.HexCoord.z);
        foreach (Vector2Int direction in CloserThan(unit.sightDistance)) {
            foreach (Hex hex in HexesAt(unitFlatPos + direction)) {
                if (Sees(unit, hex)) {
                    unit.hexesSeen.Add(hex);
                }
            }
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
