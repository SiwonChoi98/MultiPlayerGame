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
    public Dictionary<GunIconType, Sprite> GunIconDic = new Dictionary<GunIconType, Sprite>();
    
    public Dictionary<PoolObjectType, BasePoolObject> EffectVfxDic = new Dictionary<PoolObjectType, BasePoolObject>();
    public Dictionary<PoolObjectType, AudioClip> EffectAudioClipDic = new Dictionary<PoolObjectType, AudioClip>();

    public Dictionary<AudioType, AudioClip> GameAudioDic = new Dictionary<AudioType, AudioClip>();
    public Dictionary<PoolObjectType, BasePoolObject> DamageTextDic = new Dictionary<PoolObjectType, BasePoolObject>();
    
    //Resources Path
    private const string _gunDataPath = "GunData/";
    private const string _gunSpritePath = "GunSprite/Weapons";
    private const string _gunAudioPath = "GunAudio/";
    private const string _gunVfxPath = "GunVfx/";
    private const string _gunIconPath = "UIGunIcon/";
    
    private const string _effectVfxPath = "EffectVfx/";
    private const string _effectDataPath = "EffectData/EffectData";
    private const string _effectSoundPath = "EffectAudio/";

    private const string _gameAudioDataPath = "GameAudioData/GameAudioData";
    private const string _gameAudioBGMPath = "GameAudio/BGM/";
    private const string _gameAudioSFXPath = "GameAudio/SFX/";

    private const string _damageTextPath = "DamageTxt/DamageTxt";
    protected override void Awake()
    {
        base.Awake();
        
        Init_GunData();
        Init_EffectData();
        InitGameAudio();

        Init_DamageText();
    }

    private void Init_DamageText()
    {
        BasePoolObject damageText = Resources.Load<BasePoolObject>(_damageTextPath);
        DamageTextDic[PoolObjectType.DAMAGE_DEFAULT_TEXT] = damageText;
    }
    private void Init_GunData()
    {
        WeaponData_ScriptableObject[] gunDatas = Resources.LoadAll<WeaponData_ScriptableObject>(_gunDataPath);
        Sprite[] gunSprites = Resources.LoadAll<Sprite>(_gunSpritePath);
        AudioClip[] gunAudioClips = Resources.LoadAll<AudioClip>(_gunAudioPath);
        BasePoolObject[] gunVfxList = Resources.LoadAll<BasePoolObject>(_gunVfxPath);
        Sprite[] gunIconSprites = Resources.LoadAll<Sprite>(_gunIconPath);
        
        foreach (WeaponData_ScriptableObject gunData in gunDatas)
        {
            WeaponData_ScriptableObject gunDataType = gunData as WeaponData_ScriptableObject;
            
            if (gunDataType == null)
                return;
            
            GunDataDic[gunDataType.GunDataType] = gunData;

            Init_GunSprite(gunSprites, gunDataType);
            Init_GunAudio(gunAudioClips, gunDataType);
            Init_GunVfx(gunVfxList, gunDataType);
            Init_GunIcon(gunIconSprites, gunDataType);
        }
    }

    private void Init_GunSprite(Sprite[] gunSprites, WeaponData_ScriptableObject data)
    {
        foreach (Sprite gunSprite in gunSprites)
        {
            if (gunSprite.name == data.GunSpriteName)
            {
                GunSpriteDic[data.GunSpriteType] = gunSprite;
            }
        }
    }
    
    private void Init_GunIcon(Sprite[] iconSprites, WeaponData_ScriptableObject data)
    {
        foreach (Sprite iconSprite in iconSprites)
        {
            if (iconSprite.name == data.GunIconName)
            {
                GunIconDic[data.GunIconType] = iconSprite;
            }
        }
    }

    private void Init_GunAudio(AudioClip[] gunAudioClips, WeaponData_ScriptableObject data)
    {
        foreach (AudioClip gunAudioClip in gunAudioClips)
        {
            if (gunAudioClip.name == data.GunAudioName)
            {
                GunAudioClipDic[data.GunAudioType] = gunAudioClip;
            }
        }
    }

    private void Init_GunVfx(BasePoolObject[] gunVfxList, WeaponData_ScriptableObject data)
    {
        foreach (BasePoolObject gunVfx in gunVfxList)
        {
            if (gunVfx.name == data.VfxObjectName)
            {
                GunVfxDic[data.VfxObjectType] = gunVfx;
            }
        }
    }
    
    private void Init_EffectData()
    {
        EffectData_ScriptableObject effectData = Resources.Load<EffectData_ScriptableObject>(_effectDataPath);

        if (effectData == null)
            return;

        EffectVfxDic[effectData.HealthEffectType] = FindEffectVfx(effectData.HealthEffectName);
        EffectVfxDic[effectData.AddScoreEffectType] = FindEffectVfx(effectData.AddScoreEffectName);
        EffectVfxDic[effectData.AddWeaponEffectType] =  FindEffectVfx(effectData.AddWeaponEffectName);
        EffectVfxDic[effectData.ResurrectionEffectType] =  FindEffectVfx(effectData.ResurrectionEffectName);
        EffectVfxDic[effectData.HitDamageEffectType] =  FindEffectVfx(effectData.HitEffectName);
        EffectVfxDic[effectData.DeadEffectType] =  FindEffectVfx(effectData.DeadEffectName);
        
        
        EffectAudioClipDic[effectData.HealthSoundType] = FindAudioClip(effectData.HealthSoundName, _effectSoundPath);
        EffectAudioClipDic[effectData.AddScoreSoundType] = FindAudioClip(effectData.AddScoreSoundName, _effectSoundPath);
        EffectAudioClipDic[effectData.AddWeaponSoundType] = FindAudioClip(effectData.AddWeaponSoundName, _effectSoundPath);
        EffectAudioClipDic[effectData.ResurrectionSoundType] = FindAudioClip(effectData.ResurrectionSoundName, _effectSoundPath);
        EffectAudioClipDic[effectData.HitDamageSoundType] = FindAudioClip(effectData.HitSoundName, _effectSoundPath);
    }

    private BasePoolObject FindEffectVfx(string effectName)
    {
        BasePoolObject[] effectList = Resources.LoadAll<BasePoolObject>(_effectVfxPath);
        foreach (BasePoolObject effectObject in effectList)
        {
            if (effectObject.name == effectName)
            {
                return effectObject;
            }
        }

        return null;
    }

    private AudioClip FindAudioClip(string objectName, string path)
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(path);
        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip.name == objectName)
            {
                return audioClip;
            }
        }

        return null;
    }

    private void InitGameAudio()
    {
        AudioData_ScriptableObject gameAudioData = Resources.Load<AudioData_ScriptableObject>(_gameAudioDataPath);

        GameAudioDic[gameAudioData.LobbyBgmType] = FindAudioClip(gameAudioData.LobbyBgmName, _gameAudioBGMPath);
        GameAudioDic[gameAudioData.GameRoomBgmType] = FindAudioClip(gameAudioData.GameRoomBgmName, _gameAudioBGMPath);
        GameAudioDic[gameAudioData.GamePlayBgmType] = FindAudioClip(gameAudioData.GamePlayBgmName, _gameAudioBGMPath);
        
        GameAudioDic[gameAudioData.EndGameSfxType] = FindAudioClip(gameAudioData.EndGameSfxName, _gameAudioSFXPath);
        GameAudioDic[gameAudioData.ShowRankSfxType] = FindAudioClip(gameAudioData.ShowRankSfxName, _gameAudioSFXPath);
    }
}
