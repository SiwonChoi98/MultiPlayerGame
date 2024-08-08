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
    
    [Header("InGameUI")] 
    //시간
    private double timeOffset = 0;
    [SerializeField] private Text _playTimeText;
    
    [SerializeField] private RankUI _rankUI;
    [SerializeField] private EndGameUI _endGameUI;

    private void Awake()
    {
        Init_GamePlayUI();
    }
    
    private void Start()
    {
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
        UpdateUI();
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
        }
        else
        {
            _rankUI.gameObject.SetActive(true);
            _rankUI.SetRankUI(BattleManager.Instance.ManagedPlayers.Count);
        } 
    }

    public void ShowEndUI(bool isEnd)
    {
        if (isEnd)
        {
            _endGameUI.gameObject.SetActive(true);
            if (isServer)
            {
                _endGameUI.RpcSet1thUserInfo();
            }
            
        }
    }
    private void UpdateUI()
    {
        // 서버로부터 동기화된 시간을 사용하여 오프셋 계산
        double serverTime = BattleManager.Instance.PlayTime;
        double clientTime = NetworkTime.time;
        timeOffset = serverTime - clientTime;
        
        _playTimeText.text = ((int)GetCurrentTime()).ToString();
    }
    
    private double GetCurrentTime()
    {
        return NetworkTime.time + timeOffset;
    }
}
