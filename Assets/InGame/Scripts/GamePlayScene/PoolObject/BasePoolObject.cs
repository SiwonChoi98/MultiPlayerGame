using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class BasePoolObject : NetworkBehaviour
{
    protected PoolObjectType _poolObjectType;

    public void InitType(PoolObjectType poolObjectType)
    {
        this._poolObjectType = poolObjectType;
    }
    
    protected void ReturnToPool()
    {
        BasePoolObject basePoolObject = gameObject.GetComponent<BasePoolObject>();
        PoolManager.Instance.ReturnToPool(_poolObjectType, basePoolObject);
    }
}
