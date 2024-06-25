using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected WeaponData_ScriptableObject Weapon_Data;
    [SerializeField] protected BasePoolObject _fireVfx; //이펙트
    [SerializeField] protected AudioClip _audioClip; //오디오

    [SerializeField] private Transform _firePos;
    
    private const float _maxDistance = 15f;

    //오디오 재생기
    protected AudioSource _audioSource;
    protected SpriteRenderer _weaponSpriteRenderer;
    
    protected virtual void Awake()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        _weaponSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Start()
    {
        InitData();
    }

    protected void InitData()
    {
        transform.position = Weapon_Data.WeaponPos;
        _weaponSpriteRenderer.sprite = Weapon_Data.WeaponSprite;
    }
    
    public virtual void Fire()
    {
        if(isLocalPlayer)
        {
            // local predict
            PlayFireAudio();
            SpawnFireVFX();
        }

        if(isServer)
        {
            RpcPlayAudio(true);
            RpcPlayVFX(true);

            Server_HitCheck();
        }
    }
    
    //사운드
    [ClientRpc]
    private void RpcPlayAudio(bool bIsPredict)
    {
        if (bIsPredict && isOwned)
        {
            return;
        }

        PlayFireAudio();
    }

    private void PlayFireAudio()
    {
        _audioSource.PlayOneShot(_audioClip);
    }

    protected abstract PoolObjectType GetVFXPoolObjectType();

    //이펙트
    [ClientRpc]
    protected void RpcPlayVFX(bool bIsPredict)
    {
        if (bIsPredict && isOwned)
        {
            return;
        }

        SpawnFireVFX();
    }

    private void SpawnFireVFX()
    {
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(GetVFXPoolObjectType(), _fireVfx, _firePos);

        //해당 오브젝트 타입 결정
        basePoolObject.GetComponent<BasePoolObject>().InitType(GetVFXPoolObjectType());
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
        hit_statusComponent.Server_TakeDamage(10);

        /*if (isLocalPlayer) //이 주석 처리를 하면 다른 클라가 서버를 쏘면 동기화 되는데 서버가 다른 클라를 쏘면 동기화가 안됨
        {
            ApplyDamage(target, 10);
        }

        if (isServer) //이 주석 처리를 하면 서버일 떄는 서버인 애가 다른 클라를 쐈을 때 동기화 되는게 보이는데 다른 클라가 서버인 애를 쏘면 동기화 안됨
        {
            // 타겟 클라이언트에게만 히트 결과 전송
            TargetApplyDamage(target.connectionToClient, target, 10);
        }*/
    }

    /*[TargetRpc] // 특정 클라이언트에게만 이 RPC를 보냄
    private void TargetApplyDamage(NetworkConnection targetConnection, GamePlayerController target, int damage)
    {
        target.TakeDamage(damage);
    }
    
    private void ApplyDamage(GamePlayerController target, int damage)
    {
        target.TakeDamage(damage);
    }*/
}
