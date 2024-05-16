using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public abstract class AbstarctNpc : MonoBehaviour {
    [SerializeField] protected HexGrid grid;

    protected Unit unit;

    public event Action<AbstarctNpc> EndTurn = delegate { };
    protected virtual void Start() {
        unit = GetComponent<Unit>();
        if(grid == null) {
            grid = GameObject.FindAnyObjectByType<HexGrid>();
        }
    }

    public abstract void TakeTurn();

    protected virtual void OnEndTurn() { 
        EndTurn(this);
    }
}
