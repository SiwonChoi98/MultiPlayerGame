using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private InputField _roomNameField;
    public void Btn_CreateRoom()
    {
        CreateRoom();
    }

    public void Btn_JoinRoom()
    {
        JoinRoom();
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

        //System.Uri hostUri = new System.Uri(_nameInputField.text);
        manager.StartClient(); //hostUri
    }
}
