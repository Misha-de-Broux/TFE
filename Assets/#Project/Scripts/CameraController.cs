using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] float speed = 1;
    [SerializeField] int maxZoom = 12, minZoom = 6;
    public Transform target;
    Camera controlledCamera;
    InputAction move;
    // Start is called before the first frame update
    void Start()
    {
        InputActionMap map = inputActions.FindActionMap("Camera");
        map.Enable();
        move = map.FindAction("Move");
        InputAction zoom = map.FindAction("Zoom");
        zoom.performed += Zoom;
        map.FindAction("Center").performed += Center;
        controlledCamera = GetComponent<Camera>();
        if (target)
            transform.position = target.position - transform.forward * 15;
    }
    public void SetTarget(GameObject gameObjectTargetted) {
        target = gameObjectTargetted.transform;
    }
    private void Center(InputAction.CallbackContext ctx) {
        if (target)
            transform.position = target.position - transform.forward * 15;
    }

    private void Zoom(InputAction.CallbackContext ctx) {
        if (ctx.ReadValue<float>() < 0) {
            if (controlledCamera.orthographicSize > minZoom) {
                controlledCamera.orthographicSize--;
            }
        } else {
            if (controlledCamera.orthographicSize < maxZoom) {
                controlledCamera.orthographicSize++;
            }
        }
    }

    private void Move(InputAction.CallbackContext ctx) {
        
    }

    private void Update() {
        Vector2 direction = move.ReadValue<Vector2>();
        controlledCamera.transform.Translate(Time.deltaTime * speed *controlledCamera.orthographicSize / controlledCamera.orthographicSize * direction);
    }

}
