using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private InputField _roomNameField;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(AudioType.LOBBY_BGM, 0.2f, true);
    }

    public void Btn_CreateRoom()
    {
        CreateRoom();
    }

    public void Btn_JoinRoom()
    {
        JoinRoom();
    }

    public void Btn_FindMyAddress()
    {
        Application.OpenURL("https://ip.pe.kr");
    }
    //서버 생성 (방 생성)
    private void CreateRoom()
    {
        var manager = RoomManager.singleton;
        manager.StartHost();
    }
    
    
    //test (방 입장)
    private void JoinRoom()
    {
        var manager = RoomManager.singleton;
        
        manager.networkAddress = _roomNameField.text;
        //System.Uri hostUri = new System.Uri("http://"+_roomNameField.text);
        manager.StartClient(); //hostUri
    }
    
}
