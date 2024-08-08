using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PosValueTuple
{
    public Vector3 Item1;
    public bool Item2;

    public PosValueTuple(Vector3 item1, bool item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}
