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
        BattleManager.Instance.Server_UpdateManagedPlayerScore(target.netId, _scoreAmount);
        target.AddScoreTargetEffectAndSound(target.netId);
        
        BattleManager.Instance.Server_RemoveManagedItem(this);
    }
}
