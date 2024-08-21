using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StatusComponent : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = (int)Mathf.Max(0.0f, value);
    }

    public Action<StatType> UpdateHealth;
    
    [SyncVar]
    [SerializeField] private int _maxHealth;
    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = (int)Mathf.Max(0.0f, value);
    }

    [SyncVar(hook = nameof(OnDeadChanged))]
    [SerializeField] private bool _isDead = false;
    public bool IsDead => _isDead;
    [SyncVar]
    [SerializeField] private float _deadTime;

    public float DeadTime => _deadTime;
    
    //Only AI ----------------------------------------------------------
    [SyncVar(hook = nameof(OnCombatChanged))]
    private bool _isCombat = false;
    public bool IsCombat { get => _isCombat; set => _isCombat = value; }
    public Action<StatType> UpdateCombat;

    public bool IsAI = false;
    //------------------------------------------------------------------
    
    [SerializeField] private AudioSource _statusAudioSource;
    public override void OnStartServer() 
    {
        Server_SetHealth(100, 100);
        Server_SetDeadTime(3);
    }
    
    [Server]
    public void Server_SetHealth(int curHealth, int maxHealth)
    {
        if (isServer)
        {
            CurrentHealth = curHealth;
            MaxHealth = maxHealth;
        }
    }

    [Server]
    private void Server_SetDeadTime(float time)
    {
        if (isServer)
        {
            _deadTime = time;
        }
    }
    
    public void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealth?.Invoke(StatType.HEALTH);
    }
    
    private void OnDeadChanged(bool oldValue, bool newValue)
    {
        UpdateHealth?.Invoke(StatType.DEAD);
    }
    private void OnCombatChanged(bool oldValue, bool newValue)
    {
        //UpdateCombat.Invoke(StatType.COMBAT);
    }
    //공격받기
    [Server]
    public void Server_TakeDamage(int amount, uint shotPlayerNetId = 0)
    {
        if (CurrentHealth <= 0)
            return;
        
        CurrentHealth -= amount;

        RpcSpawnDamageText(amount);
        
        RpcSpawnHitEffect();
        RpcPlaySoundHit_OnlyLocalPlayer();
        
        if (shotPlayerNetId == 0)
        {
            CheckDead();
        }
        else
        {
            CheckDead(shotPlayerNetId);
        }
    }
    
    //체력회복
    [Server]
    public void Server_AddHealth(int amount)
    {
        if (CurrentHealth >= MaxHealth)
            return;

        CurrentHealth += amount;
        if (CurrentHealth > 100)
        {
            CurrentHealth = 100;
        }
        
        RpcSpawnHealthEffect();
        RpcPlaySoundHealth_OnlyLocalPlayer();
    }

    [ClientRpc]
    private void RpcSpawnHealthEffect()
    {
        SpawnEffect(PoolObjectType.HEALTH_PICKUP_EFFECT);
    }
    
    [ClientRpc]
    private void RpcSpawnHitEffect()
    {
        SpawnEffect(PoolObjectType.HIT_EFFECT);
    }
    
    [ClientRpc]
    private void RpcSpawnResurrectionEffect()
    {
        SpawnEffect(PoolObjectType.RESURRECTION_EFFECT);
    }
    [ClientRpc]
    private void RpcSpawnDeadEffect()
    {
        SpawnEffect(PoolObjectType.DEAD_EFFECT);
    }
    
    [ClientRpc]
    private void RpcSpawnDamageText(int damage)
    {
        BasePoolObject text = DataManager.Instance.DamageTextDic[PoolObjectType.DAMAGE_DEFAULT_TEXT];
        
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.DAMAGE_DEFAULT_TEXT, text,
            transform.position, Quaternion.identity);
        basePoolObject.GetComponent<BasePoolObject>().InitObjectType(PoolObjectType.DAMAGE_DEFAULT_TEXT);
        
        DamageText damageText = basePoolObject as DamageText;
        damageText.SetDamage(damage);
        
    }
    private void SpawnEffect(PoolObjectType poolObjectType)
    {
        BasePoolObject vfx = DataManager.Instance.EffectVfxDic[poolObjectType];
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(poolObjectType, vfx,
            transform.position, transform.rotation);
        
        basePoolObject.GetComponent<BasePoolObject>().InitObjectType(poolObjectType);
        
    }
    
    //본인 플레이어만 적용
    [ClientRpc]
    private void RpcPlaySoundHit_OnlyLocalPlayer()
    {
        if (!isLocalPlayer)
            return;
        
        PlayAudio(PoolObjectType.HIT_SOUND);
    }
    
    [ClientRpc]
    private void RpcPlaySoundHealth_OnlyLocalPlayer()
    {
        if (!isLocalPlayer)
            return;
        
        PlayAudio(PoolObjectType.HEALTH_PICKUP_SOUND);
    }
    
    [ClientRpc]
    private void RpcPlaySoundResurrection_OnlyLocalPlayer()
    {
        if (!isLocalPlayer)
            return;
        
        PlayAudio(PoolObjectType.RESURRECTION_SOUND);
    }
    
    public void PlayAudio(PoolObjectType poolObjectType)
    {
        AudioClip audioClip = DataManager.Instance.EffectAudioClipDic[poolObjectType];
        if (_statusAudioSource == null)
            return;
        _statusAudioSource.PlayOneShot(audioClip);
    }

    
    public void AddScoreTargetEffectAndSound(uint netId)
    {
        NetworkIdentity networkIdentity = BattleManager.Instance.Server_FindPlayer(netId);

        RpcAddScoreSpawnEffect(networkIdentity);
        RpcAddScorePlaySound_OnlyLocalPlayer(networkIdentity);
        
    }
    
    [ClientRpc]
    private void RpcAddScoreSpawnEffect(NetworkIdentity targetIdentity)
    {
        BasePoolObject vfx = DataManager.Instance.EffectVfxDic[PoolObjectType.ADDSCORE_PICKUP_EFFECT];
        BasePoolObject basePoolObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.ADDSCORE_PICKUP_EFFECT, vfx,
            targetIdentity.transform.position, targetIdentity.transform.rotation);
        
        basePoolObject.GetComponent<BasePoolObject>().InitObjectType(PoolObjectType.ADDSCORE_PICKUP_EFFECT);
    }
    
    [ClientRpc]
    private void RpcAddScorePlaySound_OnlyLocalPlayer(NetworkIdentity targetIdentity)
    {
        if (!targetIdentity.isLocalPlayer)
            return;
        
        PlayAudio(PoolObjectType.ADDSCORE_PICKUP_SOUND);
    }
    
    private void CheckDead(uint shotPlayerNetId = 0)
    {
        if (CurrentHealth > 0)
            return;
        
        _isDead = true;
        
        RpcSpawnDeadEffect();
        
        //AI -> PLAYER
        if (shotPlayerNetId == 0)
        {
            BattleManager.Instance.Server_PlayerDead(netId);
            return;
        }

        AddScoreTargetEffectAndSound(shotPlayerNetId);
        
        //AI 사망
        if (IsAI)
        {
            BattleManager.Instance.Server_AIDead(gameObject);
            BattleManager.Instance.Server_UpdateManagedPlayerScore(shotPlayerNetId, 1);;
            return;
        }
        
        //PLAYER -> PLAYER
        BattleManager.Instance.Server_PlayerDead(netId);
        BattleManager.Instance.Server_UpdateManagedPlayerScore(shotPlayerNetId, 3);;
    }
    
    [Server]
    public void Server_Resurrection()
    {
        Server_SetHealth(100, 100);
        Server_SetDeadTime(3);

        
        MoveComponent moveComponent = GetComponent<MoveComponent>();
        moveComponent.Server_GetResurrectionPosition();

        RpcSpawnResurrectionEffect();
        RpcPlaySoundResurrection_OnlyLocalPlayer();
        
        _isDead = false;
    }
}
