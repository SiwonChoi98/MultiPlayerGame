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
    public BasePoolObject SpawnFromPool(PoolObjectType poolObjectType, BasePoolObject poolObject, Vector3 position, Quaternion rotation)
    {
        if (PoolDictionary.TryGetValue(poolObjectType, out Queue<BasePoolObject> queue))
        {
            if (queue.Count > 0)
            {
                BasePoolObject poolObj = DequeuePoolObject(poolObjectType);
                
                poolObj.gameObject.SetActive(true);
                
                poolObj.transform.position = position;
                poolObj.transform.rotation = rotation;
                
                return poolObj;
            }
            else
            {
                return CreatePoolObject(poolObject, position, rotation);   
            }
        }
        else
        {
            return CreatePoolObject(poolObject, position, rotation);
        }
    }

    //해제
    public void ReturnToPool(PoolObjectType poolObjectType, BasePoolObject poolObject)
    {
        if (PoolDictionary.ContainsKey(poolObjectType))
        {
            EnqueuePoolObject(poolObjectType, poolObject);
        }
        else
        {
            // 키가 없는 경우 새로운 큐를 만들어 추가
            PoolDictionary[poolObjectType] = new Queue<BasePoolObject>();
            EnqueuePoolObject(poolObjectType, poolObject);
        }
        poolObject.gameObject.SetActive(false);
    }
    
    //private---------------------------------------------------
    
    private BasePoolObject CreatePoolObject(BasePoolObject poolObject, Vector3 pos, Quaternion rotation)
    {
        BasePoolObject obj = Instantiate(poolObject, pos, rotation);
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
