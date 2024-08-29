using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomUI : Singleton<GameRoomUI>
{
    [SerializeField] private InputField _selectUserNameInput;
    
    [SerializeField] private List<RectTransform> _colorButtonPos;
    [SerializeField] private Image _selectColorImage;

    [SerializeField] private List<RectTransform> _weaponButtonPos;
    [SerializeField] private Image _selectWeaponImage;

    [Header("User")]
    [SerializeField] private RoomPlayer _localRoomPlayer;
    [SerializeField] private List<RoomUserInfoItem> _userItemList;
    private void Start()
    {
        SoundManager.Instance.PlayBGM(AudioType.LOBBY_BGM, 0.2f, true);
    }
    
    public void Btn_SelectUserName()
    {
        var manager = NetworkRoomManager.singleton as NetworkRoomManager;

        for (int i = 0; i < manager.roomSlots.Count; i++)
        {
            if (manager.roomSlots[i].isOwned)
            {
                manager.roomSlots[i].GetComponent<RoomPlayer>().CmdSetUserName(_selectUserNameInput.text);
            }
        }
    }

    public void Btn_SelectUserColor(int color)
    {
        var manager = NetworkRoomManager.singleton as NetworkRoomManager;

        for (int i = 0; i < manager.roomSlots.Count; i++)
        {
            if (manager.roomSlots[i].isOwned)
            {
                manager.roomSlots[i].GetComponent<RoomPlayer>().CmdSetUserColor(color);
            }
        }

        _selectColorImage.transform.position = _colorButtonPos[color].transform.position;
    }

    public void Btn_SelectUserWeapon(int index)
    {
        var manager = NetworkRoomManager.singleton as NetworkRoomManager;

        for (int i = 0; i < manager.roomSlots.Count; i++)
        {
            if (manager.roomSlots[i].isOwned)
            {
                manager.roomSlots[i].GetComponent<RoomPlayer>().CmdSetUserWeapon(index);
            }
        }

        _selectWeaponImage.transform.position = _weaponButtonPos[index].transform.position;
    }

    public void AddLocalRoomPlayer(RoomPlayer roomPlayer)
    {
        _localRoomPlayer = roomPlayer;
    }

    public void Btn_Ready()
    {
        if (!_localRoomPlayer)
            return;
        
        _localRoomPlayer.DrawPlayerReadyButton();
    }

    public void SetUserItemList(bool enable, int index, string name, bool state, bool isLocalPlayer)
    {
        _userItemList[index].gameObject.SetActive(enable);
        
        if (!enable)
            return;
        
        _userItemList[index].SetPlayerInfo(name, state, isLocalPlayer);
    }
}
