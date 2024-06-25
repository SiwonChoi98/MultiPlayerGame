using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform playerTransform;
    public int depth = -10;
    
    public void SetTarget(Transform target)
    {
        playerTransform = target;
    }
    private void LateUpdate()
    {
        if (playerTransform == null)
            return;
        
        CameraMove();
    }

    private void CameraMove()
    {
        transform.position = playerTransform.position + new Vector3(0,0,depth);
    }
}
