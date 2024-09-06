using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ChaseTarget_Task : Action
{
    /// <summary>
    /// 타겟이 있으면 타겟으로 이동
    /// </summary>
    
    //타겟
    public SharedTransform STarget;
    //이동 컴포넌트
    private AgentMovement _agentMovement;
    
    public override void OnStart()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agentMovement.SetPlayPosition();
    }

    public override TaskStatus OnUpdate()
    {
        if (STarget.Value == null)
            return TaskStatus.Failure;
        //이동
        _agentMovement.SetAgentPosition(STarget.Value.position);

        return TaskStatus.Success;
    }
}
