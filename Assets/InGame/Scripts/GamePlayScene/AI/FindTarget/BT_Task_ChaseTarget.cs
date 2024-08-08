using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class BT_Task_ChaseTarget : Action
{
    //공격 가능 거리
    private float _targetAttackDistance = 4f;
    
    public SharedBool IsPatrol;
    public SharedTransform closestTarget;
    private AgentMovement _agentMovement;
    private StatusComponent _statusComponent;
    public override void OnStart()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agentMovement.SetPlayPosition();
        
        _statusComponent = GetComponent<StatusComponent>();
        _statusComponent.IsCombat = true;
    }

    public override TaskStatus OnUpdate()
    {
        //이동
        _agentMovement.SetAgentPosition(closestTarget.Value.position);
        
        //공격 사거리 체크
        float distance = Vector3.Distance(closestTarget.Value.position, transform.position);
        if (distance <= _targetAttackDistance)
        {
            _agentMovement.SetStopPosition();
            IsPatrol.Value = false;
            return TaskStatus.Failure;
        }
        
        return TaskStatus.Success;
    }
}
