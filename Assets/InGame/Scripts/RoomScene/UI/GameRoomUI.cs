using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomUI : MonoBehaviour
{
    [SerializeField] private InputField _selectUserNameInput;
    [SerializeField] private List<RectTransform> _colorButtonPos;
    [SerializeField] private Image _selectColorImage;
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
}
