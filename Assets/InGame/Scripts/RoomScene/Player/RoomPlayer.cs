using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class RoomPlayer : NetworkRoomPlayer
{
   [Header("유저 정보")]
   [SyncVar]
   [SerializeField] private Color _userColor;
   public Color UserColor => _userColor;
   
   [SyncVar]
   [SerializeField] private string _userName;
   public string UserName => _userName;
   
   [Command]
   public void CmdSetUserName(string name)
   {
      _userName = name;
   }

   public void SetUserName(string name)
   {
      _userName = name;
   }

   [Command]
   public void CmdSetUserColor(int color)
   {
      var manager = RoomManager.singleton as RoomManager;
      
      _userColor = manager.SetColor(color);
   }
   
   
}
