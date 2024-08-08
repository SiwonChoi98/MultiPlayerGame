using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnItems : MonoBehaviour
{
    [SerializeField] private List<BaseItem> _nonWeaponItemList;
    public List<BaseItem> WeaponItemList;
    
    public BasePoolObject Server_SpawnHealthItem(Vector3 pos)
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.HEALTH_ITEM, _nonWeaponItemList[0], pos, Quaternion.identity);
        if (basePoolObject != null)
        {
            NetworkServer.Spawn(basePoolObject.gameObject);
            basePoolObject.InitObjectType(PoolObjectType.HEALTH_ITEM);
        }

        return basePoolObject;
    }

    public BasePoolObject Server_SpawnAddScoreItem(Vector3 pos)
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.ADDSCORE_ITEM, _nonWeaponItemList[1], pos, Quaternion.identity);
        if (basePoolObject != null)
        {
            NetworkServer.Spawn(basePoolObject.gameObject);
            basePoolObject.InitObjectType(PoolObjectType.ADDSCORE_ITEM);
        }

        return basePoolObject;
    }
    
    public BasePoolObject Server_SpawnGunItem(int curIndex, Vector3 pos)
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(GetPoolObjectType(curIndex),
            WeaponItemList[curIndex], pos, Quaternion.identity);

        if (basePoolObject != null)
        {
            NetworkServer.Spawn(basePoolObject.gameObject);
            basePoolObject.InitObjectType(GetPoolObjectType(curIndex));

        }

        return basePoolObject;
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
