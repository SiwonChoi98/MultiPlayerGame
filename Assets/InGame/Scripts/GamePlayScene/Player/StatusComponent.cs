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

    public Action<StatType> UpdateStat;
    
    [SyncVar]
    [SerializeField] private int _maxHealth;
    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = (int)Mathf.Max(0.0f, value);
    }

    [SyncVar(hook = nameof(OnIsDeadChanged))]
    private bool _isDead = false;
    public bool IsDead
    {
        get => _isDead;
    }
    private void Start()
    {
        Server_InitHealth(70, 100);
    }

    [Server]
    private void Server_InitHealth(int curHealth, int maxHealth)
    {
        if (isServer)
        {
            CurrentHealth = curHealth;
            MaxHealth = maxHealth;
        }
    }
    
    public void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateStat?.Invoke(StatType.HEALTH);
    }
    
    private void OnIsDeadChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            UpdateStat?.Invoke(StatType.HEALTH);
        }
    }
    
    //공격받기
    [Server]
    public void Server_TakeDamage(int amount)
    {
        if (CurrentHealth <= 0)
            return;
        
        CurrentHealth -= amount;

        CheckDead();
    }

    //체력회복
    [Server]
    public void Server_AddHealth(int amount)
    {
        if (CurrentHealth >= MaxHealth)
            return;

        CurrentHealth += amount;
    }

    private void CheckDead()
    {
        if (CurrentHealth <= 0)
        {
            _isDead = true;
            
            gameObject.SetActive(false);
        }
        
    }
}
