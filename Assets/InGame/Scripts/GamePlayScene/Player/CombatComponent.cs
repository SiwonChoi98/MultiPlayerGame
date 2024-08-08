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
    public Action<StatType> UpdateBullet;
    
    [SerializeField] private AudioSource _weaponAudioSource;
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

            Server_HitCheck();
            
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
        
        //무기 바꾸고 나서 바로 공격가능한 상태로 전환
        SuccessFireReady();
        ChargeBullet();
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
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(Weapon_Data.VfxObjectType, _fireVfx, _firePos.position, _firePos.rotation);

        //해당 오브젝트 타입 결정
        basePoolObject.GetComponent<BasePoolObject>().InitObjectType(Weapon_Data.VfxObjectType);
    }
    
    
    [Server]
    public void Server_HitCheck()
    {
        Debug.DrawRay(_firePos.position, _firePos.right * _fireDistance, Color.red, 2f);
        RaycastHit2D hit = Physics2D.Raycast(_firePos.position, _firePos.right, _fireDistance);

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

    //Player -> AI --------------------------------------------------------------
    [Server]
    private void Server_PlayerToPlayerHit(RaycastHit2D hit)
    {
        var target = hit.transform.GetComponent<GamePlayerController>();
        
        if (target == null)
            return;
        
        var hit_statusComponent = hit.transform.GetComponent<StatusComponent>();
        var user_info = GetComponent<InGameUserInfo>();
        
        hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage, user_info);
    }

    [Server]
    private void Server_PlayerToAIHit(RaycastHit2D hit)
    {
        var target = hit.transform.GetComponent<AIPoolObject>();
        
        if (target == null)
            return;
        
        var hit_statusComponent = hit.transform.GetComponent<StatusComponent>();
        hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage);
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
            hit_statusComponent.Server_TakeDamage(Weapon_Data.Damage);
            Debug.Log("AI Fire-Success");
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
        return Weapon_Data.FireLoadingRate;
    }
}
