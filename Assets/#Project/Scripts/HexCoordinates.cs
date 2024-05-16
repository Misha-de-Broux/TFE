using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    public static float xOffset = 1.73f, yOffset = 0.5f, zOffest = 1.5f;

    [Header("Offset coordinates")]
    [SerializeField]
    private Vector3Int _offsetCoordinates;

    internal Vector3Int HexCoords {  get { return _offsetCoordinates; }  }

    private void Awake() {
        UpdateCoord();
    }

    private Vector3Int ConvertPositionToCoordinate(Vector3 position) {
        int z = Mathf.RoundToInt(position.z / zOffest);
        int x = Mathf.RoundToInt(position.x/xOffset - z/2f);
        int y = Mathf.RoundToInt(position.y/yOffset);
        return new Vector3Int(x, y, z);
    }

    public void UpdateCoord() {
        _offsetCoordinates = ConvertPositionToCoordinate(transform.position);
    }
}
