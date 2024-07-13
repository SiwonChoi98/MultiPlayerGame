using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BaseItem : BasePoolObject
{
    [Server]
    protected virtual void Server_PickupItem(StatusComponent target)
    {
    }

    [Command]
    private void CmdPickupItem(StatusComponent target)
    {
        PickupItem(target);
    }
    
    protected void PickupItem(StatusComponent target)
    {
        if (target == null)
            return;
        
        /*if (target.isServer)
        {*/
            Server_PickupItem(target);
            
            NetworkServer.UnSpawn(gameObject);
            ReturnToPool();  
        //}
        
        /*if (!target.isServer && target.isOwned) //
        {
            CmdPickupItem(target);
        }*/
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
