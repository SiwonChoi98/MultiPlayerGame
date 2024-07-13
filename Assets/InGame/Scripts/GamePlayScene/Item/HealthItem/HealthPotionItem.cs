using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HealthPotionItem : BaseItem
{
    [SerializeField] private int _healthAmount;
    
    [Server]
    protected override void Server_PickupItem(StatusComponent target)
    {
        target.Server_AddHealth(_healthAmount);
    }
}
