using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CombatComponent : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnDataChanged))]
    public GunDataType GunDataType;
    
    private WeaponData_ScriptableObject Weapon_Data = null;
    
    private BasePoolObject _fireVfx; //이펙트
    private AudioClip _audioClip; //오디오
    
    [SerializeField] private Transform _firePos; //공격 이펙트 위치
    
    private const float _maxDistance = 15f;
    
    
    [SyncVar]
    [SerializeField] private bool _isFireReady = false;
    [SyncVar]
    [SerializeField] private float _fireRate = 0f;
    
    [SerializeField] private AudioSource _weaponAudioSource;
    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;

    #region UnityMethod

    private void FixedUpdate()
    {
        if (isServer)
        {
            FireRateCalculate();
        }
    }

    #endregion
    
    //public--------------------------------------------------------------
    public void EquipWeaponData(GunDataType gunDataType)
    {
        GunDataType = gunDataType;
    }
    
    public void Fire()
    {
        if (!_isFireReady)
            return;
        
        if(isLocalPlayer)
        {
            // local predict
            PlayFireAudio();
            SpawnFireVFX();
        }

        if(isServer)
        {
            RpcPlayFireAudio(true);
            RpcSpawnFireVFX(true);

            Server_HitCheck();
            
            _isFireReady = false;
        }
        else if(isOwned && !isServer)
        {
            CmdFire();
        }
    }
    
    //private--------------------------------------------------------------
    private void OnDataChanged(GunDataType oldValue, GunDataType newValue)
    {
        Weapon_Data = DataManager.Instance.GunDataDic[newValue];

        if (Weapon_Data == null)
            return;
        
        _weaponSpriteRenderer.sprite = DataManager.Instance.GunSpriteDic[Weapon_Data.GunSpriteType];
        _weaponSpriteRenderer.transform.localPosition = Weapon_Data.WeaponPos;
        
        _audioClip = DataManager.Instance.GunAudioClipDic[Weapon_Data.GunAudioType];
        _fireVfx = DataManager.Instance.GunVfxDic[Weapon_Data.VfxObjectType];
        
        //무기 바꾸고 나서 바로 공격가능한 상태로 전환
        _isFireReady = true; 
        _fireRate = Weapon_Data.FireRate;
    }
    
    [Command]
    private void CmdFire()
    {
        Fire();
    }
    
    //사운드
    [ClientRpc]
    private void RpcPlayFireAudio(bool bIsPredict)
    {
        if (bIsPredict && isOwned)
        {
            return;
        }

        PlayFireAudio();
    }

    private void PlayFireAudio()
    {
        _weaponAudioSource.PlayOneShot(_audioClip);
    }

    //이펙트
    [ClientRpc]
    private void RpcSpawnFireVFX(bool bIsPredict)
    {
        if (bIsPredict && isOwned)
        {
            return;
        }

        SpawnFireVFX();
    }

    private void SpawnFireVFX()
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(Weapon_Data.VfxObjectType, _fireVfx, _firePos);

        //해당 오브젝트 타입 결정
        basePoolObject.GetComponent<BasePoolObject>().InitType(Weapon_Data.VfxObjectType);
    }
    
    [Server]
    private void Server_HitCheck()
    {
        Debug.DrawRay(_firePos.position, _firePos.right * _maxDistance, Color.red, 2f);
        RaycastHit2D hit = Physics2D.Raycast(_firePos.position, _firePos.right, _maxDistance);

        if (!hit)
            return;

        var target = hit.transform.GetComponent<GamePlayerController>();
        
        if (target == null)
            return;

        var hit_statusComponent = hit.transform.GetComponent<StatusComponent>();
        hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage);
    }

    private void FireRateCalculate()
    {
        if (_isFireReady || Weapon_Data == null)
            return;
        
        _fireRate -= Time.fixedDeltaTime;
        if (_fireRate <= 0)
        {
            _fireRate = Weapon_Data.FireRate;
            _isFireReady = true;
        }
    }
}
