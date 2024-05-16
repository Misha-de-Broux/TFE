using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public abstract class AbstarctNpc : MonoBehaviour {
    [SerializeField] UnitManager unitManager;

    Unit unit;
    void Start() {
        unit = GetComponent<Unit>();
        if(unitManager == null) {
            unitManager = GameObject.FindAnyObjectByType<UnitManager>();
        }
    }

    public abstract void TakeTurn();
}
