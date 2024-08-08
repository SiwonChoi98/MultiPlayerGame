using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class BT_Task_HasTarget : Action
{
    public SharedTransformList SharedTransformList;
    public SharedVector3 InitPos;
    public override void OnStart()
    {
        SetFindPlayer();
        SetInitPos();
    }
    
    public override TaskStatus OnUpdate()
    {
        return SharedTransformList.Value.Count > 0 ?
                TaskStatus.Success :
                TaskStatus.Failure;
    }

    private void SetFindPlayer()
    {
        // 게임 시작 시 플레이어 오브젝트 찾기
        GamePlayerController[] PlayerControllers = GameObject.FindObjectsOfType<GamePlayerController>();
        SharedTransformList.Value = new List<Transform>();
        
        foreach (var PlayerController in PlayerControllers)
        {
            SharedTransformList.Value.Add(PlayerController.transform);
        }
    }

    private void SetInitPos()
    {
        InitPos = transform.position;
    }
}
