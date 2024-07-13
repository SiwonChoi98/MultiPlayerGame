using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnItems : MonoBehaviour
{
    [SerializeField] private List<BaseItem> _healthPotionItemList;
    public List<BaseItem> GunItemList;
    
    public void Server_SpawnHealthItem()
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.HEALTH_ITEM, _healthPotionItemList[0], transform);
        if (basePoolObject != null)
        {
            NetworkServer.Spawn(basePoolObject.gameObject);
            basePoolObject.InitType(PoolObjectType.HEALTH_ITEM);
        }
    }

    public void Server_SpawnGunItem(int curIndex)
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(GetPoolObjectType(curIndex),
            GunItemList[curIndex], transform);

        if (basePoolObject != null)
        {
            NetworkServer.Spawn(basePoolObject.gameObject);
            basePoolObject.InitType(GetPoolObjectType(curIndex));

        }
    }
    
    private PoolObjectType GetPoolObjectType(int index)
    {
        switch (index)
        {
            case 0:
                return PoolObjectType.SEMIAUTO_ITEM;
            case 1:
                return PoolObjectType.SHOTGUN_ITEM;
            case 2:
                return PoolObjectType.WINCHESTER_ITEM;
            default:
                break;
        }

        return 0;
    }
}
