using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class WinchesterItem : BaseItem
{
    protected override void Server_PickupItem(StatusComponent target)
    {
        CombatComponent combatComponent = target.GetComponent<CombatComponent>();
        combatComponent.EquipWeaponData(GunDataType.WINCHESTER_DATA);
        
        BattleManager.Instance.Server_RemoveManagedItem(this);
    }
}
