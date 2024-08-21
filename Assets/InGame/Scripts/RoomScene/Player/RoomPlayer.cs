using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
public class RoomPlayer : NetworkRoomPlayer
{
   [Header("유저 정보")]
   [SyncVar]
   [SerializeField] private Color _userColor;
   public Color UserColor => _userColor;

   [SyncVar] 
   [SerializeField] private GunDataType _userWeapon;

   public GunDataType UserWeapon => _userWeapon;
   
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
   
   [Command]
   public void CmdSetUserWeapon(int weapon)
   {
      var manager = RoomManager.singleton as RoomManager;
      
      _userWeapon = manager.SetWeapon(weapon);
   }
   
   public override void DrawPlayerReadyState()
   {
      if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE"))
      {
         GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
      }
   }
   
   public override void OnGUI()
   {
      if (!showRoomGUI)
         return;

      NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
      if (room)
      {
         if (!room.showRoomGUI)
            return;

         if (!Utils.IsSceneActive(room.RoomScene))
            return;

         if (isLocalPlayer)
         {
            GameRoomUI.Instance.AddLocalRoomPlayer(this);
         }
         
         DrawPlayerReadyState();
         
         GameRoomUI.Instance.SetUserItemList(index, _userName, readyToBegin, isLocalPlayer);
      }
      
   }
}
