using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchesterItem : BaseItem
{
    protected override void Server_PickupItem(StatusComponent target)
    {
        CombatComponent combatComponent = target.GetComponent<CombatComponent>();
        combatComponent.EquipWeaponData(GunDataType.WINCHESTER_DATA);
    }
}
