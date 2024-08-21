using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EffectData", menuName = "Scriptable Object Asset/EffectData")]
public class EffectData_ScriptableObject : ScriptableObject
{
    //Name은 Resources - Object Name과 동일해야함
    
    [Header("점수획득------------------------")]
    public PoolObjectType AddScoreEffectType;
    public string AddScoreEffectName;

    public PoolObjectType AddScoreSoundType;
    public string AddScoreSoundName;
    
    [Header("체력회복------------------------")]
    public PoolObjectType HealthEffectType;
    public string HealthEffectName;
    
    public PoolObjectType HealthSoundType;
    public string HealthSoundName;
    
    [Header("무기획득------------------------")]
    public PoolObjectType AddWeaponEffectType;
    public string AddWeaponEffectName;
    
    public PoolObjectType AddWeaponSoundType;
    public string AddWeaponSoundName;
    
    [Header("부활------------------------")]
    public PoolObjectType ResurrectionEffectType;
    public string ResurrectionEffectName;
    
    public PoolObjectType ResurrectionSoundType;
    public string ResurrectionSoundName;
    
    [Header("히트------------------------")]
    public PoolObjectType HitDamageEffectType;
    public string HitEffectName;
    
    public PoolObjectType HitDamageSoundType;
    public string HitSoundName;
    
    [Header("사망------------------------")]
    public PoolObjectType DeadEffectType;
    public string DeadEffectName;
}
