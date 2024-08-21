using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerCanvas : CharacterCanvas
{
    [Header("UserID")] 
    [SerializeField] private InGameUserInfo _userInfo;
    [SerializeField] private Text _userIdText;
    [Header("BULLET")]
    [SerializeField] private Text _currentBulletCountText;
    [SerializeField] private Image _bulletCountImage;
    [SerializeField] private GameObject _bulletImageObject;
    [SerializeField] private Image _bulletChargeProgress;

    [Header("MINIMAP")] 
    [SerializeField] private Image _localMinimapImage; 
    
    [Header("POPUP")]
    [SerializeField] private Dead_UI _deadUI;

    [Header("MANAGED SPRITE")]
    [SerializeField] private List<GameObject> _managedSprites;
    
    protected override void Start()
    {
        base.Start();
        
        _userInfo.UpdateUserID += SetUserInfo;
        _userInfo.OnUserIDChanged("_", "_");
        
        _combatComponent.UpdateBullet += UpdateStat;
        _combatComponent.OnBulletCountChanged(_combatComponent.BulletCount, _combatComponent.BulletCount);
        
        HideImageToNotLocalPlayer();
        SetColorToNotLocalPlayer();
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        _combatComponent.UpdateBullet -= UpdateStat;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        UpdateFireLoadingRate();
    }
    
    protected override void UpdateStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.HEALTH:
                UpdateHealth();
                break;
            case StatType.BULLET:
                UpdateBullet();
                break;
            case StatType.DEAD:
                
                if (_statusComponent.IsDead) SetManagedSprite(false);
                else SetManagedSprite(true);
                
                if (!_statusComponent.isLocalPlayer)
                    return;

                if (_statusComponent.IsDead)
                {
                    ShowDeadUI();
                }
                else
                {
                    HideDeadUI();
                }
                    
                break;
        }
    }
    
    private void UpdateBullet()
    {
        _currentBulletCountText.text = _combatComponent.BulletCount.ToString();
        _bulletCountImage.fillAmount = (float)_combatComponent.BulletCount / (float)_combatComponent.MaxBulletCount;
    }
    
    private void UpdateFireLoadingRate()
    {
        if (_combatComponent.IsFireReady)
        {
            if (_bulletChargeProgress.fillAmount != 0)
                _bulletChargeProgress.fillAmount = 0f;
            
            return;
        }
            
        _bulletChargeProgress.fillAmount = _combatComponent.FireLoadingRate / _combatComponent.GetMaxFireLoadingRate();
    }
    
    private void ShowDeadUI()
    {
        _deadUI.gameObject.SetActive(true); 
    }

    private void HideDeadUI()
    {
        _deadUI.gameObject.SetActive(false); 
    }
    private void HideImageToNotLocalPlayer()
    {
        if (!_combatComponent.isLocalPlayer)
        {
            _bulletChargeProgress.gameObject.SetActive(false);
            _currentBulletCountText.gameObject.SetActive(false);
            _bulletImageObject.SetActive(false);
        }
    }

    private void SetColorToNotLocalPlayer()
    {
        if (!_statusComponent.isLocalPlayer)
        {
            //_localMinimapImage.color = Color.yellow;
            _localMinimapImage.gameObject.SetActive(false);
        }
    }

    private void SetManagedSprite(bool isActive)
    {
        for (int i = 0; i < _managedSprites.Count; i++)
        {
            _managedSprites[i].SetActive(isActive);
        }

        //only local player
        if (_statusComponent.isLocalPlayer)
        {
            _bulletImageObject.SetActive(isActive);
        }
    }

    private void SetUserInfo()
    {
        if (_userInfo == null)
            return;
        
        _userIdText.text = _userInfo.UserName;
        
        //0번이 body sprite
        _managedSprites[0].GetComponent<SpriteRenderer>().color = _userInfo.UserColor;
    }
    
}
