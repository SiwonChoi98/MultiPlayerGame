using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CombatComponent : NetworkBehaviour
{
    [SerializeField] private Weapon _weapon;

    public void EquipWeapon(Weapon weapon)
    {
        this._weapon = weapon;
    }

    public void Fire()
    {
        if (_weapon == null)
            return;

        _weapon.Fire();
        if(isOwned && !isServer)
        {
            CmdFire();
        }
    }

    [Command]
    private void CmdFire()
    {
        Fire();
    }
    
}
