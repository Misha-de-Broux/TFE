using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
[RequireComponent(typeof(HexHighlight))]
[RequireComponent (typeof(HexCoordinates))]
public class Hex : MonoBehaviour {
    HexHighlight highlight;
    private HexCoordinates hexCoordinates;
    [SerializeField]public  bool isObstacle = false, isWalkable = true;
    [SerializeField] int _cost = 2;

    private CapsuleCollider seenCollider;
    public int Cost { get { return _cost; } }
    public int _seenBy = 0;
    [SerializeField] private Hex _covered;
    private Boolean _discovered = false;
    public event Action onCover, onReveal;
    public event Action<bool> onHide;
    public bool Hidden { get; private set; }

    public Vector3Int HexCoords => hexCoordinates.HexCoords;

    private void Awake() {
        hexCoordinates = GetComponent<HexCoordinates>();
        highlight = GetComponent<HexHighlight>();
        seenCollider = GetComponent<CapsuleCollider>();
        Hidden = true;
        foreach (Renderer renderer in GetComponentsInChildren<MeshRenderer>()) {
            renderer.enabled = false;
        }
    }
    private void Start() {
        RaycastHit hit;
        if(_seenBy == 0) {
            onHide?.Invoke(true);
        }
        if (Physics.Raycast(transform.position - transform.up * 0.25f, -transform.up,out hit, 1f)) {
            _covered = hit.collider.GetComponent<Hex>();
            _covered.Cover();
        }
    }

    public void EnnableHighlight() { 
        highlight.SetGlow(true);
    }

    public void DisableHighlight() {
        highlight.SetGlow(false);
    }

    public void Cover() {
        onCover?.Invoke();
    }

    public void Reveal() {
        onReveal?.Invoke() ;
    }

    internal void ResetHighlight() {
        highlight.ResetGlowHightLight();
    }

    internal void HighLightPath() {
        highlight.HighLightPath();
    }

    internal void HighlightColor(Color color) {
        highlight.HighLightColor(color);
    }

    public void See() {
        Discover();
        if(_seenBy == 0) {
            Hidden = false;
            Hide(Hidden);
        }
        _seenBy++;
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.up * 0.25f, -transform.up, out hit, 1f)) {
            _covered = hit.collider.GetComponent<Hex>();
            _covered.Cover();
        }
        _covered?.See();
    }

    private void Discover() {
        if(!_discovered) {
            _discovered = true;
            foreach (Renderer renderer in GetComponentsInChildren<MeshRenderer>()) {
                renderer.enabled = true;
            }
        }
    }

    public void UnSee() {
        _seenBy--;
        if(_seenBy == 0) {
            Hidden = true;
            Hide(Hidden);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.up * 0.25f, -transform.up, out hit, 1f)) {
            _covered = hit.collider.GetComponent<Hex>();
            _covered.Cover();
        }
        _covered?.UnSee();
    }

    private void Hide(bool hidden) {
        highlight.Hide(hidden);
        onHide?.Invoke(hidden);
    }

    public float IsVisible(bool isVisible) {
        seenCollider.enabled = isVisible;
        return seenCollider.height;
    }
}
