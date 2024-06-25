using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : Weapon
{
    protected override PoolObjectType GetVFXPoolObjectType()
    {
        return PoolObjectType.SHOTGUN_VFX;
    }
}
