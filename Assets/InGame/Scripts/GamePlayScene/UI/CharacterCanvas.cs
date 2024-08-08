using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterCanvas : MonoBehaviour
{
    [SerializeField] protected StatusComponent _statusComponent;
    [SerializeField] protected CombatComponent _combatComponent;
    
    [Header("UI")]
    [SerializeField] private Image _currentHealthImage;
    [SerializeField] private Text _currentHealthText;
    
    protected virtual void Start()
    {
        _statusComponent.UpdateHealth += UpdateStat;
        _statusComponent.OnHealthChanged(_statusComponent.CurrentHealth, _statusComponent.MaxHealth);
    }

    protected virtual void OnDestroy()
    {
        _statusComponent.UpdateHealth -= UpdateStat;
    }

    protected virtual void LateUpdate()
    {
    }
    
    protected virtual void UpdateStat(StatType statType)
    {
    }
    
    protected void UpdateHealth()
    {
        _currentHealthText.text = _statusComponent.CurrentHealth.ToString();
        _currentHealthImage.fillAmount = (float)_statusComponent.CurrentHealth / (float)_statusComponent.MaxHealth;
    }
    
   
}
