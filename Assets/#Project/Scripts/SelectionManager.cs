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

    private bool IsHex(GameObject result) {
        return result.GetComponent<Hex>() != null; 
    }

    private bool FindObject(Vector3 mousePosition, out GameObject result) {
        float minimumDistance = float.MaxValue;
        result = null;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        foreach(RaycastHit hit in Physics.RaycastAll(ray, float.MaxValue, SelectMask)) {
            GameObject go = hit.collider.gameObject;
            Debug.Log(go);
            if (IsUnit(go))
            {
                result = go;
                return true;
            }
            if (IsHex(go)) {
                if (go.GetComponent<Hex>().isWalkable) {
                    float distance = Vector3.Distance(go.transform.position, hit.point);
                    if (distance < minimumDistance) {
                        minimumDistance = distance;
                        result = go;
                    }
                }
            }
        }
        return result != null;
    }
}
