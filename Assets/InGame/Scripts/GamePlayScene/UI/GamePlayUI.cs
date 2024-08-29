using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GamePlayUI : NetworkBehaviour
{
    public static GamePlayUI Instance = null;
    
    [Header("Time")] 
    private double timeOffset = 0;
    [SerializeField] private Text _playTimeText;

    [Header("UserWeapon")]
    [SerializeField] private Image _userWeaponImage;
    [SerializeField] private Image _userWeaponStatDamageImage;
    [SerializeField] private Image _userWeaponStatFireLoadingImage;
    [SerializeField] private Text _userWeaponDamageText;
    [SerializeField] private Text _userWeaponFireLoadingText;
    
    
    [SerializeField] private RankUI _rankUI;
    [SerializeField] private EndGameUI _endGameUI;

    [SerializeField] private Text _showRankDescText;
    
    private void Awake()
    {
        Init_GamePlayUI();
    }
    
    private void Start()
    {
        //SoundManager.Instance.PlayBGM(AudioType.GAMEPLAY_BGM, 0.2f, true);
        SoundManager.Instance.StopBGM();
        BattleManager.Instance.UpdateEnd += ShowEndUI;
    }
    
    private void OnDestroy()
    {
        // 객체가 파괴될 때 이벤트에서 메서드를 제거
        BattleManager.Instance.UpdateEnd -= ShowEndUI;
    }
    
    private void Update()
    {
        if(BattleManager.Instance.IsEnd)
            return;
        
        UpdateTimeUI();
    }

    private void Init_GamePlayUI()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject); 
        }
    }
    public void ShowRankUI()
    {
        if (_rankUI.gameObject.activeSelf)
        {
            _rankUI.gameObject.SetActive(false);
            _showRankDescText.gameObject.SetActive(true);
        }
        else
        {
            _rankUI.gameObject.SetActive(true);
            _showRankDescText.gameObject.SetActive(false);
            SoundManager.Instance.PlaySFX(AudioType.SHOWRANKUI_SFX, 0.7f);
        } 
    }

    public void ShowEndUI(bool isEnd)
    {
        if (isEnd)
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlaySFX(AudioType.ENDGAME_SFX, 0.4f);
            _endGameUI.gameObject.SetActive(true);
            if (isServer)
            {
                _endGameUI.RpcSet1thUserInfo();
            }
            
        }
    }
    private void UpdateTimeUI()
    {
        // 서버로부터 동기화된 시간을 사용하여 오프셋 계산
        double serverTime = BattleManager.Instance.PlayTime;
        double clientTime = NetworkTime.time;
        timeOffset = serverTime - clientTime;
        
        _playTimeText.text = ((int)GetCurrentTime()).ToString();
    }

    public void UpdateUserWeaponUI(Sprite gunIcon, int userWeaponDamage, float userWeaponFireLoaing)
    {
        _userWeaponDamageText.text = userWeaponDamage.ToString();
        _userWeaponFireLoadingText.text = ((int)userWeaponFireLoaing).ToString();

        _userWeaponStatDamageImage.fillAmount = (float)userWeaponDamage / 20; //최대치 변수로 수정 필요
        _userWeaponStatFireLoadingImage.fillAmount = userWeaponFireLoaing / 3.5f; //최대치 변수로 수정 필요 

        _userWeaponImage.sprite = gunIcon;
    }
    
    private double GetCurrentTime()
    {
        return NetworkTime.time + timeOffset;
    }
}
