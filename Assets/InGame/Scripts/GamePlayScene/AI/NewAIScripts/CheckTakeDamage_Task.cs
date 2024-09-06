using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;

public class CheckTakeDamage_Task : Action
{
    /// <summary>
    /// 데미지 받았는지 체크 후 위치 지정
    /// </summary>
    
    //공격받은 위치
    public SharedVector3 TakeDamagedPos;
    
    public override TaskStatus OnUpdate()
    {
        CheckTakeDamaged();
        return TaskStatus.Success;
    }

    private void CheckTakeDamaged()
    {
        StatusComponent statusComponent = GetComponent<StatusComponent>();
        
        if (statusComponent.IsAIDamaged)
        {
            TakeDamagedPos.Value = statusComponent.ShotPlayerPos;
            statusComponent.IsAIDamaged = false;
        }
    }
    
}
