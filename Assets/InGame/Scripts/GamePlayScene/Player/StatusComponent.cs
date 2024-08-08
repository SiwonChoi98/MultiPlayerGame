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
    public void Server_TakeDamage(int amount, InGameUserInfo info = null)
    {
        if (CurrentHealth <= 0)
            return;
        
        CurrentHealth -= amount;

        if (info == null)
        {
            CheckDead();
        }
        else
        {
            CheckDead(info);
        }
    }
    
    //체력회복
    [Server]
    public void Server_AddHealth(int amount)
    {
        if (CurrentHealth >= MaxHealth)
            return;

        CurrentHealth += amount;
    }

    private void CheckDead(InGameUserInfo info = null)
    {
        if (CurrentHealth > 0)
            return;
        
        _isDead = true;
        
        if (IsAI)
        {
            BattleManager.Instance.Server_AIDead(gameObject);
        }
        else
        {
            if (info != null)
            {
                info.Server_AddScore(3); //죽었을때 데미지 준 유저에게 점수 올림
                BattleManager.Instance.Server_PlayerDead(netId);
            }
        }
        
        
    }
    
    public void Server_Resurrection()
    {
        _isDead = false;
        
        Server_SetHealth(100, 100);
        Server_SetDeadTime(3);

        
        MoveComponent moveComponent = GetComponent<MoveComponent>();
        moveComponent.Server_GetResurrectionPosition();
    }
}
