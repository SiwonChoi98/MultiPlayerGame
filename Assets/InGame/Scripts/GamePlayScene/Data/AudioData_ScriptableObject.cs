using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameAudioData", menuName = "Scriptable Object Asset/GameAudioData")]
public class AudioData_ScriptableObject : ScriptableObject
{
    [Header("BGM------------------------")]
    public AudioType LobbyBgmType;
    public string LobbyBgmName;
    
    public AudioType GameRoomBgmType;
    public string GameRoomBgmName;
    
    public AudioType GamePlayBgmType;
    public string GamePlayBgmName;
    
    [Header("SFX------------------------")]
    public AudioType ShowRankSfxType;
    public string ShowRankSfxName;
    
    public AudioType EndGameSfxType;
    public string EndGameSfxName;
}
