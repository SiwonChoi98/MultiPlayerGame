using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RoomManager : NetworkRoomManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        
        TempSetUserName(conn);
    }

    private void TempSetUserName(NetworkConnectionToClient conn)
    {
        if (conn.identity == null)
            return;
        
        var roomPlayer = conn.identity.GetComponent<RoomPlayer>();
        if (roomPlayer != null)
        {
            roomPlayer.SetUserName(NetworkRoomManager.singleton.numPlayers.ToString());
        }
    }

    public Color SetColor(int colorIndex)
    {
        switch (colorIndex)
        {
            case 0:
                return Color.white;
            case 1:
                return Color.red;
            case 2:
                return Color.blue;
            case 3:
                return Color.green;
        }

        return Color.black;
    }

    public GunDataType SetWeapon(int gunIndex)
    {
        switch (gunIndex)
        {
            case 0:
                return GunDataType.SEMIAUTORIFLE_DATA;
            case 1:
                return GunDataType.SHOTGUN_DATA;
            case 2:
                return GunDataType.WINCHESTER_DATA;
        }

        return GunDataType.SEMIAUTORIFLE_DATA;
    }
}
