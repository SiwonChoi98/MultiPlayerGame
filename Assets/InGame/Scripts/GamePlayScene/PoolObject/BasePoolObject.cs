using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class BasePoolObject : NetworkBehaviour
{
    [SerializeField] protected PoolObjectType _poolObjectType;
    [SerializeField] protected int _spawnPosIndex;
    
    public void InitObjectType(PoolObjectType poolObjectType)
    {
        _poolObjectType = poolObjectType;
    }

    [Server]
    public PoolObjectType Server_GetObjectType()
    {
        return _poolObjectType;
    }

    [Server]
    public void Server_SetSpawnPosIndex(int spawnPosIndex)
    {
        _spawnPosIndex = spawnPosIndex;
    }
    
    [Server]
    public int Server_GetSpawnPosIndex()
    {
        return _spawnPosIndex;
    }
    
    protected void ReturnToPool()
    {
        BasePoolObject basePoolObject = gameObject.GetComponent<BasePoolObject>();
        PoolManager.Instance.ReturnToPool(_poolObjectType, basePoolObject);
    }
}
