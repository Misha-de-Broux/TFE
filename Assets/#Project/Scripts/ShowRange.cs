using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class ShowRange : MonoBehaviour {
    public UnityEvent clearSelection;
    public Unit captain;
    [SerializeField] InputActionAsset uiActions;
    [SerializeField] Color rangeColor;
    private List<Hex> highlightedHexes = new List<Hex>();
    void Start() {
        InputActionMap uiMap = uiActions.FindActionMap("UI");
        uiMap.Enable();
        InputAction showRange = uiMap.FindAction("ShowRange");
        showRange.performed += HighLightRange;
        showRange.canceled += ClearHighLightRange;
    }

    private void ClearHighLightRange(InputAction.CallbackContext ctx) {
        foreach (Hex hex in highlightedHexes) {
            hex.DisableHighlight();
        }
    }

    void HighLightRange(InputAction.CallbackContext ctx) {
        highlightedHexes.Clear();
        clearSelection?.Invoke();
        foreach (Hex hex in captain.orderRange) {
            if (hex.isWalkable) {
                hex.HighlightColor(rangeColor);
                highlightedHexes.Add(hex);
            }
        }
    }
}
