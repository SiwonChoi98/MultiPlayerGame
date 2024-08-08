using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotateCanvas : MonoBehaviour
{
    private void LateUpdate()
    {
        FreezeRotate_Canvas();
    }
    
    private void FreezeRotate_Canvas()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }
}
