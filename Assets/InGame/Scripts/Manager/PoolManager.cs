using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<PoolObjectType, Queue<BasePoolObject>> PoolDictionary = new Dictionary<PoolObjectType, Queue<BasePoolObject>>();
    
    
    //public-----------------------------------------------------------------------
    
    //생성
    public BasePoolObject SpawnFromPool(PoolObjectType poolObjectType, BasePoolObject poolObject, Transform pos)
    {
        if (PoolDictionary.TryGetValue(poolObjectType, out Queue<BasePoolObject> queue))
        {
            if (queue.Count > 0)
            {
                BasePoolObject poolObj = DequeuePoolObject(poolObjectType);
                
                poolObj.gameObject.SetActive(true);
                
                poolObj.transform.position = pos.transform.position;
                poolObj.transform.rotation = pos.transform.rotation;
                
                return poolObj;
            }
            else
            {
                return CreatePoolObject(poolObject, pos);   
            }
        }
        else
        {
            return CreatePoolObject(poolObject, pos);
        }
    }

    //해제
    public void ReturnToPool(PoolObjectType poolObjectType, BasePoolObject poolObject)
    {
        if (PoolDictionary.ContainsKey(poolObjectType))
        {
            poolObject.gameObject.SetActive(false);
            EnqueuePoolObject(poolObjectType, poolObject);
        }
        else
        {
            // 키가 없는 경우 새로운 큐를 만들어 추가
            PoolDictionary[poolObjectType] = new Queue<BasePoolObject>();
            poolObject.gameObject.SetActive(false);
            EnqueuePoolObject(poolObjectType, poolObject);
        }
        
        NetworkServer.Destroy(poolObject.gameObject);
    }
    
    //private---------------------------------------------------
    
    private BasePoolObject CreatePoolObject(BasePoolObject poolObject, Transform pos)
    {
        BasePoolObject obj = Instantiate(poolObject, pos.position, pos.rotation);
        return obj;
    }
    
    private BasePoolObject DequeuePoolObject(PoolObjectType poolObjectType)
    {
        BasePoolObject obj = PoolDictionary[poolObjectType].Dequeue();
        return obj;
    }
    
    private void EnqueuePoolObject(PoolObjectType poolObjectType, BasePoolObject poolObject)
    {
        PoolDictionary[poolObjectType].Enqueue(poolObject);
    }
}
