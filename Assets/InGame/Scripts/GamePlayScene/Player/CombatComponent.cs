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
    [SerializeField] private bool _isPlayer;

    [SyncVar(hook = nameof(OnDataChanged))]
    public GunDataType GunDataType;
    
    private WeaponData_ScriptableObject Weapon_Data = null;
    
    private BasePoolObject _fireVfx; //이펙트
    private AudioClip _audioClip; //오디오
    
    [SerializeField] private Transform _firePos; //공격 이펙트 위치
    
    private const float _fireDistance = 15f;
    
    [Header("Fire")]
    [SyncVar]
    [SerializeField] private bool _isFireReady = false;
    public bool IsFireReady => _isFireReady;
    
    [SyncVar]
    [SerializeField] private float _fireLoadingRate = 0f;
    public float FireLoadingRate => _fireLoadingRate;
    
    [SyncVar(hook = nameof(OnBulletCountChanged))]
    [SerializeField] private int _bulletCount = 0;
    public int BulletCount => _bulletCount;
    public int MaxBulletCount = 0;
    public Action<StatType> UpdateBullet;
    
    [SerializeField] private AudioSource _weaponAudioSource;
    [SerializeField] private AudioSource _statusAudioSource;
    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;

    private bool _isWall = false;
    public bool IsWall => _isWall;
    #region UnityMethod
    
    private void FixedUpdate()
    {
        if (isServer)
        {
            Server_FireRateCalculate();
        }
    }

    #endregion
    
    //public--------------------------------------------------------------
    public void EquipWeaponData(GunDataType gunDataType)
    {
        if(!isServer)
        {
            return;
        }

        GunDataType = gunDataType;

        RpcSpawnAddWeaponEffect();
        RpcPlayAudioAddWeaponOnlyLocalPlayer();
    }
    
    public void Fire()
    {
        if (!_isFireReady)
            return;
        
        if(isLocalPlayer)
        {
            // local predict
            PlayFireAudio(_audioClip);
            SpawnFireVFX();
        }

        if(isServer)
        {
            RpcPlayFireAudio(true);
            RpcSpawnFireVFX(true);

            Server_HitCheck(_fireDistance);
            
            UseBullet();
            
        }
        else if(isOwned && !isServer)
        {
            CmdFire();
        }
    }
    
    public void OnBulletCountChanged(int oldValue, int newValue)
    {
        UpdateBullet?.Invoke(StatType.BULLET);
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
        
        MaxBulletCount = Weapon_Data.BulletCount;
        
        //무기 바꾸고 나서 바로 공격가능한 상태로 전환
        SuccessFireReady();
        ChargeBullet();
        
        //update ui
        if (isLocalPlayer)
        {
            Sprite gunIcon = DataManager.Instance.GunIconDic[Weapon_Data.GunIconType];
            
            GamePlayUI.Instance.UpdateUserWeaponUI(gunIcon , Weapon_Data.Damage, Weapon_Data.FireLoadingRate);
        }
        
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

        PlayFireAudio(_audioClip);
    }

    [ClientRpc]
    private void RpcPlayAudioAddWeaponOnlyLocalPlayer()
    {
        if (!isLocalPlayer)
            return;
        
        AudioClip audioClip = DataManager.Instance.EffectAudioClipDic[PoolObjectType.ADDWEAPON_PICKUP_SOUND];
        if (_statusAudioSource == null)
            return;
        _statusAudioSource.PlayOneShot(audioClip);
    }
    
    public void PlayFireAudio(AudioClip audioClip)
    {
        _weaponAudioSource.PlayOneShot(audioClip);
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
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(Weapon_Data.VfxObjectType, _fireVfx, 
            _firePos.position, _firePos.rotation);

        //해당 오브젝트 타입 결정
        basePoolObject.GetComponent<BasePoolObject>().InitObjectType(Weapon_Data.VfxObjectType);
        
        RotatePaticleStartPosition(basePoolObject);
    }
    
    [ClientRpc]
    private void RpcSpawnAddWeaponEffect()
    {
        BasePoolObject vfx = DataManager.Instance.EffectVfxDic[PoolObjectType.ADDWEAPON_PICKUP_EFFECT];
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.ADDWEAPON_PICKUP_EFFECT, vfx,
            transform.position, transform.rotation);
        
        basePoolObject.GetComponent<BasePoolObject>().InitObjectType(PoolObjectType.ADDWEAPON_PICKUP_EFFECT);
    }

    private void RotatePaticleStartPosition(BasePoolObject basePoolObject)
    {
        if (basePoolObject == null)
        {
            Debug.LogError("BasePoolObject is null!");
            return;
        }
        
        ParticleSystem.MainModule mainModule = basePoolObject.GetComponent<ParticleSystem>().main;
        
        float zRotation = basePoolObject.transform.eulerAngles.z;
        if (zRotation > 180f) zRotation -= 360f;

        // 조정된 각도를 라디안으로 변환
        float zRotationInRadians = -zRotation * Mathf.Deg2Rad;
        mainModule.startRotation = zRotationInRadians;
    }
    
    [Server]
    public void Server_HitCheck(float fireDistance)
    {
        Debug.DrawRay(_firePos.position, _firePos.right * fireDistance, Color.red, 2f);
        RaycastHit2D hit = Physics2D.Raycast(_firePos.position, _firePos.right, fireDistance);

        if (!hit)
            return;

        if (_isPlayer)
        {
            Server_PlayerToPlayerHit(hit);
            Server_PlayerToAIHit(hit);
        }
        else
        {
            Server_AIToPlayer(hit);
        }

    }

    //Player -> Player ---------------------------------------------------------
    [Server]
    private void Server_PlayerToPlayerHit(RaycastHit2D hit)
    {
        var target = hit.transform.GetComponent<GamePlayerController>();
        
        if (target == null)
            return;
        
        var hit_statusComponent = hit.transform.GetComponent<StatusComponent>();
        hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage, ShotObjectType.PLAYERTOPLAYER, netId);
    }
    
    //Player -> AI --------------------------------------------------------------
    [Server]
    private void Server_PlayerToAIHit(RaycastHit2D hit)
    {
        var target = hit.transform.GetComponent<AIPoolObject>();
        
        if (target == null)
            return;
        
        var hit_statusComponent = hit.transform.GetComponent<StatusComponent>();
        hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage, ShotObjectType.PLAYERTOAI, netId);
    }

    //AI -> Player --------------------------------------------------------------
    private void Server_AIToPlayer(RaycastHit2D hit)
    {
        Server_AIWallCheck(hit);
        
        PlayFireAudio(_audioClip);
        var playerController = hit.transform.GetComponent<GamePlayerController>();
        if (playerController != null)
        {
            var hit_statusComponent = hit.transform.GetComponent<StatusComponent>();
            hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage, ShotObjectType.AITOPLAYER);
        }
    }
    public void Server_AIWallCheck(RaycastHit2D hit)
    {
        if (hit.transform.tag == "Wall")
        {
            _isWall = true;
        }
        else
        {
            _isWall = false;    
        }
    }
    [Server]
    private void Server_FireRateCalculate()
    {
        if (_isFireReady || Weapon_Data == null || _bulletCount > 0)
            return;
        
        _fireLoadingRate -= Time.fixedDeltaTime;
        if (_fireLoadingRate <= 0)
        {
            SuccessFireReady();
            ChargeBullet();
        }
    }

    private void UseBullet()
    {
        if (_bulletCount < 0)
            return;

        _bulletCount--;

        if (_bulletCount <= 0)
        {
            _isFireReady = false;
        }
    }

    public void MakeEmptyBullet()
    {
        _bulletCount = 0;
        _isFireReady = false;
    }

    [Command]
    public void CmdMakeEmptyBullet()
    {
        MakeEmptyBullet();
    }
    
    private void ChargeBullet()
    {
        _bulletCount = Weapon_Data.BulletCount;
    }
    
    private void SuccessFireReady()
    {
        _fireLoadingRate = Weapon_Data.FireLoadingRate;
        _isFireReady = true;
    }

    public float GetMaxFireLoadingRate()
    {
        if (Weapon_Data == null)
            return 0;
        
        return Weapon_Data.FireLoadingRate;
    }
}
