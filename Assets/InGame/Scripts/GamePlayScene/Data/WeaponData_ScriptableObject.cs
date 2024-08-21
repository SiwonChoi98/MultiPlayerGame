using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Object Asset/WeaponData")]
public class WeaponData_ScriptableObject : ScriptableObject
{
    //Name은 Resources - Object Name과 동일해야함
    
    public GunDataType GunDataType;
    
    public PoolObjectType VfxObjectType;
    public string VfxObjectName;
    
    public GunSpriteType GunSpriteType;
    public string GunSpriteName;
    
    public GunAudioType GunAudioType;
    public string GunAudioName;
    
    public Vector2 WeaponPos;

    //UI 용
    public GunIconType GunIconType;
    public string GunIconName;

    public int Damage;
    public float FireLoadingRate; //장전 쿨타임
    public int BulletCount; //총알 갯수

}
