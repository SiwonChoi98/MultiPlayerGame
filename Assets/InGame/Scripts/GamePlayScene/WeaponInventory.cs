using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

//----아직 안쓰이는 중--------------------
public class WeaponInventory : MonoBehaviour
{
    
    //public List<Weapon> Weapons = new ();

    /*public void Init_Weapon()
    {
        //_weaponObject.Weapon_Data = DataManager.Instance.GunDataDic[GunDataType.SEMIAUTORIFLE_DATA];
        _combatComponent.EquipWeapon(_weaponObject);
    }*/

   
    public void Add_Weapon(CombatComponent combatComponent)
    {
        Remove_Weapon();
        
        CombatComponent newCombatComponent = Instantiate(combatComponent);
        NetworkServer.Spawn(newCombatComponent.gameObject);
        
        SetParent_Weapon(newCombatComponent);
        
        //Weapons.Add(newWeapon);
        
        //_combatComponent.EquipWeapon(Weapons[0]);
    }

    private void Remove_Weapon(int DestroyIndex = 0)
    {
        if (DestroyIndex == 0)
        {
            //Weapons.Remove(Weapons[0]);
        }
        else
        {
            //Weapons.Remove(Weapons[DestroyIndex]);
        }
        
        CombatComponent prevCombatComponent = GetComponentInChildren<CombatComponent>();
        
        Destroy(prevCombatComponent.gameObject);
        NetworkServer.Destroy(prevCombatComponent.gameObject);
    }

    private void SetParent_Weapon(CombatComponent combatComponent)
    {
        combatComponent.transform.SetParent(gameObject.transform);
        combatComponent.transform.localScale = gameObject.transform.localScale;
        combatComponent.transform.rotation = gameObject.transform.rotation;
    }
}
