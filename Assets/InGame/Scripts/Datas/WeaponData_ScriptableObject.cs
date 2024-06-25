using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Object Asset/WeaponData")]
public class WeaponData_ScriptableObject : ScriptableObject
{
    public string Name;

    public GunDataType GunDataType;
    public Sprite WeaponSprite;
    public Vector2 WeaponPos;
    
}
