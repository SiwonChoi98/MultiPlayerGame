using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EndGameUI : NetworkBehaviour
{
    [SerializeField] private Button _serverChangeButton;
    
    [Header("UserInfo")] 
    [SerializeField] private Text _userNameText;
    [SerializeField] private Text _userScoreText;
    [SerializeField] private Image _userImage;
    
    [SyncVar(hook = nameof(OnUserNameChanged))]
    private string _userName;
    [SyncVar(hook = nameof(OnUserScoreChanged))] 
    private int _userScore = -1;
    [SyncVar(hook = nameof(OnUserColorChanged))]
    private Color _userColor;
    private void OnEnable()
    {
        ShowServerButton();
    }
    
    [ClientRpc]
    public void RpcSet1thUserInfo()
    {
        if(!NetworkServer.active)
            return;
        
        BattleManager manager = BattleManager.Instance;
        InGameUserInfo userInfo = manager.Server_FindPlayer(Server_Get1thUserNetID(manager)).GetComponent<InGameUserInfo>();
        
        _userName = userInfo.UserName;
        _userScore = userInfo.UserScore;
        _userColor = userInfo.UserColor;
    }
    
    private void OnUserNameChanged(string oldName, string newName)
    {
        _userNameText.text = newName;
    }

    private void OnUserScoreChanged(int oldScore, int newScore)
    {
        _userScoreText.text = newScore.ToString();
    }

    private void OnUserColorChanged(Color oldColor, Color newColor)
    {
        _userImage.color = newColor;
    }
    
    [Server]
    private uint Server_Get1thUserNetID(BattleManager manager)
    {
        uint targetUserNetId = 0;
        int maxScore = -1;
        foreach (var managedPlayer in manager.ManagedPlayers)
        {
            InGameUserInfo userInfo = manager.Server_FindPlayer(managedPlayer).GetComponent<InGameUserInfo>();
            if (userInfo.UserScore > maxScore)
            {
                maxScore = userInfo.UserScore;
                targetUserNetId = managedPlayer;
            }
        }
        return targetUserNetId;
    }
    private void ShowServerButton()
    {
        if (NetworkServer.active)
            _serverChangeButton.gameObject.SetActive(true);
        else
            _serverChangeButton.gameObject.SetActive(false);
    }
    public void Btn_ServerChangeRoomScene()
    {
        var manager = NetworkRoomManager.singleton as NetworkRoomManager;
        
        if (NetworkServer.active && Utils.IsSceneActive(manager.GameplayScene))
        {
            manager.ServerChangeScene(manager.RoomScene);
            
            if (Utils.IsSceneActive(manager.RoomScene))
                GUI.Box(new Rect(10f, 180f, 520f, 150f), "PLAYERS??");
        }
    }
    
    public void Btn_ServerChangeOfflineScene()
    {
        var manager = NetworkRoomManager.singleton as NetworkRoomManager;
        
        if (Utils.IsSceneActive(manager.GameplayScene))
        {
            if (NetworkServer.active)
            {
                manager.StopHost();
            }
            else
            {
                manager.StopClient();
            }
            
        }
    }
}
