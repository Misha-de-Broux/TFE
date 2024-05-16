using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SelectionManager : MonoBehaviour {
    [SerializeField] Camera mainCamera;

    public LayerMask SelectMask;



    public UnityEvent<GameObject> UnitSelected;
    public UnityEvent<GameObject> DestinationSelected;

    private void Awake() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
    }

    public void HandleClick(Vector3 mousePosition) {
        GameObject result;
        if(FindObject(mousePosition, out result)){
            if (IsUnit(result)) {
                UnitSelected?.Invoke(result);
            } else {
                DestinationSelected?.Invoke(result);
            }
        }

    }

    private bool IsUnit(GameObject result) {
        return result.GetComponent<Unit>() != null;
    }

    private bool FindObject(Vector3 mousePosition, out GameObject result) {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, float.MaxValue, SelectMask)) {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }
}
