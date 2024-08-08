using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetCamera : MonoBehaviour
{
    public Transform TargetTransform;
    private int depth = -10;
    
    public void SetTarget(Transform target)
    {
        TargetTransform = target;
    }
    private void LateUpdate()
    {
        if (TargetTransform == null)
            return;
        
        MoveCamera();
    }

    private void MoveCamera()
    {
        transform.position = TargetTransform.position + new Vector3(0,0,depth);
    }
}
