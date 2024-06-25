using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateComponent : MonoBehaviour
{
    public void RotateTowardsMouse(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }
}
