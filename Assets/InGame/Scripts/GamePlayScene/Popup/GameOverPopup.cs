using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GameOverPopup : MonoBehaviour
{
    [SerializeField] private bool _isServer = false;
    public void Init_Setting(bool isServer)
    {
        _isServer = isServer;
    }
    //로비 이동
    public void Btn_Lobby()
    {
        if (_isServer)
        {
            /*
            서버를 다른 클라에게 줘야함
            */
        }
        NetworkClient.Disconnect();
    }

    //관전
    public void Btn_Observer()
    {
        gameObject.SetActive(false);
        
    }
}
