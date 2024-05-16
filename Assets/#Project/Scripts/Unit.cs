using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
[RequireComponent(typeof(HexCoordinates))]
[RequireComponent(typeof(UnitHighight))]
public class Unit : MonoBehaviour
{
    public int mouvement = 8, sightDistance = 6;
    public float eyesHeight = 1;

    public event Action onStartingStep = () => { };
    public event Action onEndingStep = () => { };

    private HexCoordinates _hexCoordinates;
    [SerializeField] float MouvementDuration = 1, RotationDuration = 0.3f, MouvementHeight = 1;

    private UnitHighight _highlight;
    private Queue<Vector3> _path = new Queue<Vector3>();

    public event Action<Unit> MouvementFinished;

    public Vector3Int HexCoord {
        get {
            _hexCoordinates.UpdateCoord();
            return _hexCoordinates.HexCoords;
        }
    }

    private void Awake() {
        _hexCoordinates = GetComponent<HexCoordinates>();
        _highlight = GetComponent<UnitHighight>();
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
        Quaternion startRotation = transform.rotation;
        Vector3 direction = new Vector3(endPosition.x, transform.position.y, endPosition.z) - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);
        if(!Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1)) {
            float timeElapsed = 0;
            while(timeElapsed < RotationDuration) {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / RotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                yield return null;
            }
            transform.rotation = endRotation;
        }
        StartCoroutine(MouvementCoroutine(endPosition));
    }

    private IEnumerator MouvementCoroutine(Vector3 endPosition) {
        onStartingStep();
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
        transform.position = endPosition;
        onEndingStep();
        if(_path.Count > 0) {
            StartCoroutine(RotationCoroutine(_path.Dequeue()));
        } else {
            MouvementFinished?.Invoke(this);
        }
    }

    private float YMouvement(float start, float end, float step) {
        return (2*end - 2*start - 4*MouvementHeight) * Mathf.Pow(step, 2) + (start - end + 4) * step + start;
    }
}
