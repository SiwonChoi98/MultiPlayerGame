using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    //Data Dictionary
    public Dictionary<GunDataType, WeaponData_ScriptableObject> GunDataDic = new Dictionary<GunDataType, WeaponData_ScriptableObject>();
    public Dictionary<GunSpriteType, Sprite> GunSpriteDic = new Dictionary<GunSpriteType, Sprite>();
    public Dictionary<GunAudioType, AudioClip> GunAudioClipDic = new Dictionary<GunAudioType, AudioClip>();
    public Dictionary<PoolObjectType, BasePoolObject> GunVfxDic = new Dictionary<PoolObjectType, BasePoolObject>();

    //Resources Path
    private const string _gunDataPath = "GunData/";
    private const string _gunSpritePath = "GunSprite/Weapons";
    private const string _gunAudioPath = "GunAudio/";
    private const string _gunVfxPath = "GunVfx/";
    private void Start()
    {
        Init_GunData();
    }
    
    
    private void Init_GunData()
    {
        WeaponData_ScriptableObject[] gunDatas = Resources.LoadAll<WeaponData_ScriptableObject>(_gunDataPath);
        Sprite[] gunSprites = Resources.LoadAll<Sprite>(_gunSpritePath);
        AudioClip[] gunAudioClips = Resources.LoadAll<AudioClip>(_gunAudioPath);
        BasePoolObject[] gunVfxList = Resources.LoadAll<BasePoolObject>(_gunVfxPath);
        
        foreach (WeaponData_ScriptableObject gunData in gunDatas)
        {
            WeaponData_ScriptableObject gunDataType = gunData as WeaponData_ScriptableObject;
            
            if (gunDataType == null)
                return;
            
            GunDataDic[gunDataType.GunDataType] = gunData;

            Init_GunSprite(gunSprites, gunDataType);
            Init_GunAudio(gunAudioClips, gunDataType);
            Init_GunVfx(gunVfxList, gunDataType);
        }
    }

    private void Init_GunSprite(Sprite[] gunSprites, WeaponData_ScriptableObject gunDataType)
    {
        foreach (Sprite gunSprite in gunSprites)
        {
            if (gunSprite.name == gunDataType.GunSpriteName)
            {
                GunSpriteDic[gunDataType.GunSpriteType] = gunSprite;
            }
        }
    }

    private void Init_GunAudio(AudioClip[] gunAudioClips, WeaponData_ScriptableObject gunDataType)
    {
        foreach (AudioClip gunAudioClip in gunAudioClips)
        {
            if (gunAudioClip.name == gunDataType.GunAudioName)
            {
                GunAudioClipDic[gunDataType.GunAudioType] = gunAudioClip;
            }
        }
    }

    private void Init_GunVfx(BasePoolObject[] gunVfxList, WeaponData_ScriptableObject gunDataType)
    {
        foreach (BasePoolObject gunVfx in gunVfxList)
        {
            if (gunVfx.name == gunDataType.VfxObjectName)
            {
                GunVfxDic[gunDataType.VfxObjectType] = gunVfx;
            }
        }
    }
}
