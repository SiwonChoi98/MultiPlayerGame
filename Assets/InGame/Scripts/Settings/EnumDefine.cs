using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunDataType
{
    SEMIAUTORIFLE_DATA,
    WINCHESTER_DATA,
    SHOTGUN_DATA,
    DUMMY_DATA
}

public enum GunIconType
{
    SEMIAUTORIFLE_ICON,
    WINCHESTER_ICON,
    SHOTGUN_ICON
}
public enum GunSpriteType
{
    SEMIAUTORIFLE_SPRITE,
    WINCHESTER_SPRITE,
    SHOTGUN_SPRITE
}

public enum GunAudioType
{
    SEMIAUTORIFLE_AUDIO,
    WINCHESTER_AUDIO,
    SHOTGUN_AUDIO
}
public enum PoolObjectType
{
    /// <summary>
    /// 순서 보장 
    /// </summary>
    SEMIAUTORIFLE_VFX,
    WINCHESTER_VFX,
    SHOTGUN_VFX,
    
    HEALTH_ITEM,
    ADDSCORE_ITEM,
    
    SEMIAUTO_ITEM,
    WINCHESTER_ITEM,
    SHOTGUN_ITEM,
    
    AI,
    
    HEALTH_PICKUP_EFFECT,
    ADDSCORE_PICKUP_EFFECT,
    ADDWEAPON_PICKUP_EFFECT,
    RESURRECTION_EFFECT,
    HIT_EFFECT,
    
    HEALTH_PICKUP_SOUND,
    ADDSCORE_PICKUP_SOUND,
    ADDWEAPON_PICKUP_SOUND,
    RESURRECTION_SOUND,
    HIT_SOUND,
    
    DEAD_EFFECT,
    DAMAGE_DEFAULT_TEXT,
}

public enum StatType
{
    HEALTH,
    BULLET,
    COMBAT,
    DEAD,
}

public enum SpawnPostionType
{
    PLAYER,
    ITEM,
}

public enum ShotObjectType
{
    PLAYERTOPLAYER,
    PLAYERTOAI,
    AITOPLAYER,
}
public enum AudioType
{
    LOBBY_BGM,
    GAMEROOM_BGM,
    GAMEPLAY_BGM,
    
    SHOWRANKUI_SFX,
    ENDGAME_SFX
}




