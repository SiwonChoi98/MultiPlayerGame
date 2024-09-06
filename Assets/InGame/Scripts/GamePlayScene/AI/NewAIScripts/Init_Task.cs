using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
public class Init_Task : Action
{
    /// <summary>
    /// 각종 초기화 (S 붙은 필드이름은 이미 초기화 되어서 공유만 하는 필드)
    /// </summary>
    
    //시작 위치
    public SharedVector3 InitPos;
    
    //타겟을 찾을 범위
    public SharedFloat FindTargetRadius;

    //찾을 타겟 레이어마스크
    public SharedLayerMask FindTargetLayerMask;
    
    public override void OnStart()
    {
        SetInit();
    }
    
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
    
    private void SetInit()
    {
        InitPos.Value = transform.position;
    }
}
