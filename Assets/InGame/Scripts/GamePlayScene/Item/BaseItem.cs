using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BaseItem : BasePoolObject
{
    [Server]
    protected virtual void Server_FunctionToItem(StatusComponent target)
    {
    }

    [Command]
    private void CmdFunctionToItem(StatusComponent target)
    {
        FunctionToItem(target);
    }
    
    protected void FunctionToItem(StatusComponent target)
    {
        if (target.isServer)
        {
            Server_FunctionToItem(target);
        }
        
        if (!target.isServer && target.isOwned)
        {
            CmdFunctionToItem(target);
        }
    }
    
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var target = other.GetComponent<StatusComponent>();
            
            FunctionToItem(target);
            
            ReturnToPool();
        }
    }
}
