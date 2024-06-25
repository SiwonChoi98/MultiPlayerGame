using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Org.BouncyCastle.Asn1.X509;
using UnityEngine;

public class HealthPotionItem : BaseItem
{
    [SerializeField] private int _healthAmount;
    
    [Server]
    protected override void Server_FunctionToItem(StatusComponent target)
    {
        target.Server_AddHealth(_healthAmount);
    }
}
