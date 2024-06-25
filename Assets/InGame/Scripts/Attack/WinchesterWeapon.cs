using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchesterWeapon : Weapon
{
    protected override PoolObjectType GetVFXPoolObjectType()
    {
        return PoolObjectType.WINCHESTER_VFX;
    }
}
