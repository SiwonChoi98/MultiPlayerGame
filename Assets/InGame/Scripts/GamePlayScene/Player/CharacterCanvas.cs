using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterCanvas : MonoBehaviour
{
    [SerializeField] private StatusComponent _statusComponent;
    
    [Header("UI")]
    [SerializeField] private Image _currentHealthImage;
    [SerializeField] private Text _currentHealthText;

    [SerializeField] private GameOverPopup _gameOverPopup;

    private void Start()
    {
        _statusComponent.UpdateStat += UpdateStat;
        _statusComponent.OnHealthChanged(_statusComponent.CurrentHealth, _statusComponent.MaxHealth);

        SetGameOverPopup(false);
    }

    private void OnDestroy()
    {
        _statusComponent.UpdateStat -= UpdateStat;
    }

    private void LateUpdate()
    {
        FreezeRotate_Canvas();
    }
    
    public void UpdateStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.HEALTH:
                UpdateHealth();
                
                if (_statusComponent.IsDead && _statusComponent.isLocalPlayer)
                    SetGameOverPopup(true);
                
                break;
        }
    }
    
    private void UpdateHealth()
    {
        _currentHealthText.text = _statusComponent.CurrentHealth.ToString();
        _currentHealthImage.fillAmount = (float)_statusComponent.CurrentHealth / (float)_statusComponent.MaxHealth;
    }

    private void SetGameOverPopup(bool isTrue)
    {
        _gameOverPopup.gameObject.SetActive(isTrue); 
        
        if(isTrue)
            _gameOverPopup.Init_Setting(_statusComponent.isServer);
    }
    
    //다른 곳에서도 쓰이면 인터페이스로 수정
    private void FreezeRotate_Canvas()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }
}
