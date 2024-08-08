using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AddScoreItem : BaseItem
{
    [SerializeField] private int _scoreAmount;
    
    [Server]
    protected override void Server_PickupItem(StatusComponent target)
    {
        InGameUserInfo userInfo = target.GetComponent<InGameUserInfo>();
        userInfo.Server_AddScore(_scoreAmount);
        
        BattleManager.Instance.Server_RemoveManagedItem(this);
    }
}
