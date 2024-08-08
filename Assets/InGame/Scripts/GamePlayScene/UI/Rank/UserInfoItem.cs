using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UserInfoItem : MonoBehaviour
{
    [SerializeField] private Text _userName;
    [SerializeField] private Text _userScore;

    public void SetUserInfo(string userName, int userScore)
    {
        _userName.text = userName;
        _userScore.text = userScore.ToString();
    }
}
