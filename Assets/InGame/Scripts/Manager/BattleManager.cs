using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] private BasePoolObject _healthPotionItem;

    [SerializeField] private List<BasePoolObject> _gunItemlList;
    private void Update()
    {
        SpawnHealthItem();
        SpawnGunItem();
    }

    private void SpawnHealthItem()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.HEALTH_ITEM, _healthPotionItem, transform);
            NetworkServer.Spawn(basePoolObject.gameObject);
            
            //해당 오브젝트 타입 결정
            basePoolObject.GetComponent<BasePoolObject>().InitType(PoolObjectType.HEALTH_ITEM);
        }
    }

    private void SpawnGunItem()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            int index =  Random.Range(0, _gunItemlList.Count);
            //BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType, _gunItemlList[index], transform);
            //NetworkServer.Spawn(basePoolObject.gameObject);
            
            //해당 오브젝트 타입 결정
            //basePoolObject.GetComponent<BasePoolObject>().InitType(PoolObjectType.HEALTH_ITEM);
        }
    }
}
