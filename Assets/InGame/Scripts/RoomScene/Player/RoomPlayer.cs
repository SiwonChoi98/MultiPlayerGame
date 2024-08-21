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
      base.DrawPlayerReadyState();

      if (isLocalPlayer)
      {
         GUILayout.Label(_userName + " - LOCAL PLAYER -");
      }
      else
      {
         GUILayout.Label(_userName);
      }
      
      
      
      
      if (readyToBegin)
         GUILayout.Label("READY");
      else
         GUILayout.Label("NOT READY");

      if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE"))
      {
         // This button only shows on the Host for all players other than the Host
         // Host and Players can't remove themselves (stop the client instead)
         // Host can kick a Player this way.
         GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
      }
      
      GUILayout.EndArea();
   }
}
