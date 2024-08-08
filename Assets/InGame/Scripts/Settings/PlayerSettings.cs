using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings
{
    public static string NickName;
}



/// <summary>
/// 임시) 방에서 조정할 수 있게 변환 예정
/// </summary>
public class GameSettings
{
    public static float InitPlayerMoveSpeed = 2f;

    public static float InGameItemSpawnTime = 5f;
    
    /// <summary>
    // InGameDeadTime과 WInGameDeadTime 동일해야함
    /// </summary>
    public static WaitForSeconds WInGameDeadTime = new WaitForSeconds(3f);
    public static float InGameDeadTime = 3f;
}