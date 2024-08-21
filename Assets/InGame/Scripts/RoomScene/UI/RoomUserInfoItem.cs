using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUserInfoItem : MonoBehaviour
{
    [SerializeField] private Text _userNameText;
    [SerializeField] private Text _userReadyStateText;
    [SerializeField] private Text _localPlayerText;

    public void SetPlayerInfo(string name, bool state, bool isLocalPlayer)
    {
        _userNameText.text = name;
        
        if (state)
        {
            _userReadyStateText.text = "READY";
            _userReadyStateText.color = Color.green;
        }
        else
        {
            _userReadyStateText.text = "NOT READY";
            _userReadyStateText.color = Color.yellow;
        }
        

        if (isLocalPlayer)
        {
            _localPlayerText.gameObject.SetActive(true);
        }
        else
        {
            _localPlayerText.gameObject.SetActive(false);
        }
    }
}
