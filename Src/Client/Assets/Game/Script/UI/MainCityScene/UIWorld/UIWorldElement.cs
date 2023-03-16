using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : UIBase
{
    public Transform owner;
    public float height = 2.0f;

    private void Update()
    {
        if (owner != null) transform.position = owner.position + Vector3.up * height;
        if (Camera.main != null) transform.forward = Camera.main.transform.forward;
    }
}