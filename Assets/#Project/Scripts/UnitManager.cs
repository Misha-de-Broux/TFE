using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    [SerializeField] HexGrid grid;
    [SerializeField] MouvementSystem mouvementSystem;

    public bool PlayerTurn { get; private set; } = true;

    [SerializeField] Unit unit;
    Hex hex;

    public void HandleUnitSelected(GameObject unit) {
        if (PlayerTurn) {
            Unit selectedUnit = unit.GetComponent<Unit>();
            if (!IsTheSameUnitSelected(selectedUnit)) {
                PrepareUnitForMouvement(selectedUnit);
            }
        }
    }

    public void HandleHexSelected(GameObject hex) {
        if (PlayerTurn && unit != null) {
            Hex selectedHex = hex.GetComponent<Hex>();
            if(!HandleOutOfRange(selectedHex.HexCoords) && !HandleSectedHexIsUnitHex(selectedHex.HexCoords)) {
                HandleDestination(selectedHex);
            }
        }
    }

    private void HandleDestination(Hex selectedHex) {
        if(hex == null || hex != selectedHex) {
            hex = selectedHex;
            mouvementSystem.ShowPath(selectedHex.HexCoords, grid);
        } else {
            mouvementSystem.MoveUnit(unit, grid);
            PlayerTurn = false;
            unit.MouvementFinished += ResetTurn;
            ClearOldSelection();
        }
    }

    private void ResetTurn(Unit unit) {
        unit.MouvementFinished -= ResetTurn;
        PlayerTurn = true;
    }

    private bool HandleSectedHexIsUnitHex(Vector3Int hexCoords) {
        if(hexCoords == unit.HexCoord) {
            unit.Desselect();
            ClearOldSelection();
            return true;
        }
        return false;
    }

    private bool HandleOutOfRange(Vector3Int hexCoords) {
        return !mouvementSystem.IsHexInRange(hexCoords);
    }

    private void PrepareUnitForMouvement(Unit selectedUnit) {
        if(unit != null) {
            ClearOldSelection();
        }
        unit = selectedUnit;
        unit.Select();
        mouvementSystem.showRange(unit, grid);
    }

    private bool IsTheSameUnitSelected(Unit selectedUnit) {
        if (this.unit == selectedUnit) {
            ClearOldSelection();
            return true;
        }
        return false;
    }

    private void ClearOldSelection() {
        hex = null;
        unit.Desselect();
        mouvementSystem.HideRange(grid);
        unit = null;
    }
}
