using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class InGameUserInfo : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnUserIDChanged))]
    [SerializeField] private string _userName;
    public string UserName => _userName;
    public Action UpdateUserID;

    [SyncVar] 
    [SerializeField] private GunDataType _userWeapon;
    public GunDataType UserWeapon => _userWeapon;
    
    private int _userSpawnPosIndex = -1;
    
    [SyncVar] 
    [SerializeField] private Color _userColor;
    public Color UserColor => _userColor;
    
    [SerializeField] private AudioSource _statusAudioSource;
    public override void OnStartServer()
    {
        Server_SetUserName();
        Server_SetUserColor();
        Server_SetUserWeapon();
    }

    public void OnUserIDChanged(string old, string newString)
    {
        UpdateUserID?.Invoke();
    }
    private RoomPlayer Server_FindLocalRoomPlayer()
    {
        var manager = NetworkRoomManager.singleton as NetworkRoomManager;
        
        foreach (var slot in manager.roomSlots)
        {
            if (slot.connectionToClient.connectionId == connectionToClient.connectionId)
            {
                var roomPlayer = slot.GetComponent<RoomPlayer>();
                if (roomPlayer != null)
                {
                    return roomPlayer;
                }
            }
        }
        return null;
    }
    [Server]
    private void Server_SetUserName()
    {
        _userName = Server_FindLocalRoomPlayer().UserName;
    }
    [Server]
    private void Server_SetUserColor()
    {
        _userColor = Server_FindLocalRoomPlayer().UserColor;
    }

    [Server]
    private void Server_SetUserWeapon()
    {
        _userWeapon = Server_FindLocalRoomPlayer().UserWeapon;
    }
    
    [Server]
    public void Server_SetUserSpawnPosIndex(int spawnPosIndex)
    {
        _userSpawnPosIndex = spawnPosIndex;
    }

    [Server]
    public int Server_GetUserSpawnPosIndex()
    {
        return _userSpawnPosIndex;
    }
}
