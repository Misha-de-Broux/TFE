using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent<Vector3> PointerClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectMouseClick();
    }

    private void DetectMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            PointerClick?.Invoke(Input.mousePosition);
        }
    }
}
