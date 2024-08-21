using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Mirror;

public class BaseItem : BasePoolObject
{
    [Server]
    protected virtual void Server_PickupItem(StatusComponent target)
    {
    }
    
    [Server]
    protected void PickupItem(StatusComponent target)
    {
        if (target == null)
            return;
       
        //pickup function
        Server_PickupItem(target);
        
        //Item PickUp시 스폰 위치 정상화
        //BattleManager.Instance.Server_SetTuple(BattleManager.Instance.SpawnItemPosList, _spawnPosIndex, true);
        
        //return object
        NetworkServer.UnSpawn(gameObject);
        ReturnToPool();  
        
    }
    
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var target = other.GetComponent<StatusComponent>();

            if (target.isServer)
            {
                PickupItem(target);   
            }
        }
    }
}
