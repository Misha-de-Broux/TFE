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
    Camera camera;
    InputAction move;
    // Start is called before the first frame update
    void Start()
    {
        InputActionMap map = inputActions.FindActionMap("Camera");
        map.Enable();
        move = map.FindAction("Move");
        InputAction zoom = map.FindAction("Zoom");
        zoom.performed += (ctx) => Zoom(ctx);
        camera = GetComponent<Camera>();
    }

    private void Zoom(InputAction.CallbackContext ctx) {
        if (ctx.ReadValue<float>() < 0) {
            if (camera.orthographicSize > minZoom) {
                camera.orthographicSize--;
            }
        } else {
            if (camera.orthographicSize < maxZoom) {
                camera.orthographicSize++;
            }
        }
    }

    private void Move(InputAction.CallbackContext ctx) {
        
    }

    private void Update() {
        Vector2 direction = move.ReadValue<Vector2>();
        camera.transform.Translate(Time.deltaTime * speed *camera.orthographicSize / camera.orthographicSize * direction);
    }

}
