using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
[RequireComponent(typeof(HexCoordinates))]
[RequireComponent(typeof(UnitHighight))]
public class Unit : MonoBehaviour {
    public int mouvement = 8, sightDistance = 6;
    public float eyesHeight = 1;

    private HexCoordinates _hexCoordinates;
    [SerializeField] float MouvementDuration = 1, RotationDuration = 0.3f, MouvementHeight = 1;
    private LayerMask _hexMask;
    public List<Hex> hexesSeen = new List<Hex>();
    public Unit captain;
    public List<Unit> pawns = new List<Unit>();
    public List<Hex> orderRange = new List<Hex>();


    private UnitHighight _highlight;
    private Queue<Vector3> _path = new Queue<Vector3>();

    public event Action<Unit> MouvementFinished, onStartingStep, onEndingStep;

    public Vector3Int HexCoord {
        get {
            _hexCoordinates.UpdateCoord();
            return _hexCoordinates.HexCoords;
        }
    }

    private void Awake() {
        _hexCoordinates = GetComponent<HexCoordinates>();
        _highlight = GetComponent<UnitHighight>();
        _hexMask = LayerMask.GetMask(new string[] { "HexTile" });
        if (captain == null) {
            captain = this;
        } else {
            captain.pawns.Add(this);
        }
        Hex position = GetHex();
        onStartingStep += Free;
        onEndingStep += Occupy;
        if (position != null) {
            transform.position = position.transform.position;
            position.isObstacle = true;
            Occupy(this);
        } else {
            Destroy(gameObject);
        }
    }

    private Hex GetHex() {
        RaycastHit hit;
        Hex hex = null;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, float.MaxValue, _hexMask)) {
            hex = hit.collider.gameObject.GetComponent<Hex>();
        }
        return hex;
    }

    internal void Desselect() {
        _highlight.SetGlow(false);
    }
    internal void Select() {
        _highlight.SetGlow(true);
    }

    public void Move(List<Vector3> path) {
        _path = new Queue<Vector3>(path);
        Vector3 firstTarget = _path.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget));
    }

    private IEnumerator RotationCoroutine(Vector3 endPosition) {
        if (transform.position != endPosition) {
            Quaternion startRotation = transform.rotation;
            Vector3 direction = new Vector3(endPosition.x, transform.position.y, endPosition.z) - transform.position;
            Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);
            if (GetHex()?.Hidden ?? true) { transform.rotation = endRotation; }
            if (!Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1)) {
                float timeElapsed = 0;
                while (timeElapsed < RotationDuration) {
                    timeElapsed += Time.deltaTime;
                    float lerpStep = timeElapsed / RotationDuration;
                    transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                    yield return null;
                }
                transform.rotation = endRotation;
            }
        }
        StartCoroutine(MouvementCoroutine(endPosition));
    }

    private IEnumerator MouvementCoroutine(Vector3 endPosition) {
        onStartingStep(this);
        if (!GetHex().Hidden) {
            Vector3 startPosition = transform.position;
            Vector3 target = new Vector3(endPosition.x, startPosition.y, endPosition.z);
            float timeElapsed = 0;
            while (timeElapsed < MouvementDuration) {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / MouvementDuration;
                transform.position = Vector3.Lerp(startPosition, target, lerpStep);
                transform.position = new Vector3(transform.position.x, YMouvement(startPosition.y, endPosition.y, lerpStep), transform.position.z);
                yield return null;
            }
        }
        transform.position = endPosition;
        onEndingStep(this);
        if (_path.Count > 0) {
            StartCoroutine(RotationCoroutine(_path.Dequeue()));
        } else {
            MouvementFinished?.Invoke(this);
        }
    }

    public void UpdateOrderRange() {
        if (captain == this) {
            List<Hex> newOrderRange = new List<Hex>();
            newOrderRange.AddRange(hexesSeen);
            List<Unit> toCheck = new List<Unit>();
            toCheck.AddRange(pawns);
            bool updated = true;
            while (updated) {
                updated = false;
                List<Unit> update = new List<Unit>();
                foreach (Unit unit in toCheck) {
                    foreach (Hex hexToCheck in unit.hexesSeen) {
                        if (newOrderRange.Contains(hexToCheck)) {
                            update.Add(unit);
                            updated = true;
                            foreach (Hex hex in unit.hexesSeen) {
                                if (!newOrderRange.Contains(hex)) {
                                    newOrderRange.Add(hex);
                                }
                            }
                        }
                    }
                }
                foreach (Unit unit in update) {
                    toCheck.Remove(unit);
                }
            }
            orderRange = newOrderRange;
        }
    }

    private float YMouvement(float start, float end, float step) {
        return start == end ? start : (2 * end - 2 * start - 4 * MouvementHeight) * Mathf.Pow(step, 2) + (start - end + 4) * step + start;
    }

    private void Occupy(Unit unit) {
        Hex hex = unit.GetHex();
        hex.isWalkable = false;
        hex.onHide += Hide;
        Hide(hex.Hidden);
    }
    private void Free(Unit unit) {
        Hex hex = unit.GetHex();
        hex.isWalkable = true;
        hex.onHide -= Hide;
    }

    private void Hide(bool hidden) {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            renderer.enabled = !hidden;
        }
    }

    public bool IsActionable() {
        return captain.orderRange.Contains(GetHex());
    }
}
