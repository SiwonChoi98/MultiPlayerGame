using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<GunDataType, ScriptableObject> GunDataDic = new Dictionary<GunDataType, ScriptableObject>();

    private void Start()
    {
        Init_GunData();
    }

    private void Init_GunData()
    {
        ScriptableObject[] gunDatas = Resources.LoadAll<ScriptableObject>("GunData/");

        foreach (ScriptableObject gunData in gunDatas)
        {
            WeaponData_ScriptableObject gunDataType = gunData as WeaponData_ScriptableObject;
            
            if (gunDataType == null)
                return;
            
            GunDataDic[gunDataType.GunDataType] = gunData;
        }
    }
}
