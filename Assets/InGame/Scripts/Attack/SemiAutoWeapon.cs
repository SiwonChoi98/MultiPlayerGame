using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoWeapon : Weapon
{

    protected override PoolObjectType GetVFXPoolObjectType()
    {
        return PoolObjectType.SEMIAUTO_VFX;
    }


}
