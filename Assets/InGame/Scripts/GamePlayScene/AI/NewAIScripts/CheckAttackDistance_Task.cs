using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckAttackDistance_Task : Action
{
    /// <summary>
    /// 공격 사거리 체크 후 실패 성공 여부
    /// </summary>
    
    //타겟
    public SharedTransform STarget;
    
    //공격 가능 거리
    private float _targetAttackDistance = 4f;

    public override TaskStatus OnUpdate()
    {
        return CheckAttackDistance();
    }

    private TaskStatus CheckAttackDistance()
    {
        if (STarget.Value == null)
            return TaskStatus.Failure;
        
        //공격 사거리 체크
        float distance = Vector3.Distance(STarget.Value.position, transform.position);
        if (distance <= _targetAttackDistance)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
