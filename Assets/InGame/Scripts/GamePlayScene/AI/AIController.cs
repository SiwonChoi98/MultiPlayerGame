using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AIController : NetworkBehaviour
{
    private CombatComponent _combatComponent;
    private void Awake()
    {
        InitComponent();
    }
    public override void OnStartServer() 
    {
        EquipWeapon();
    }
    
    //컴포넌트 초기화
    private void InitComponent()
    {
        _combatComponent = GetComponent<CombatComponent>();
    }
    
    //무기 장착
    private void EquipWeapon()
    {
        if (_combatComponent == null)
            return;
        
        _combatComponent.EquipWeaponData(GunDataType.SEMIAUTORIFLE_DATA);
    }
}
