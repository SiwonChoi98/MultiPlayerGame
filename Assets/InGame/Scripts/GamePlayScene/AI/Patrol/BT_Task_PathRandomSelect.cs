using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class BT_Task_PathRandomSelect : Action
{
    public SharedVector3 closestTarget;
    public SharedBool IsPatrol;
    public Vector3 ranPos;
    public SharedVector3 InitPos;
    
    private float _patrolRadius = 10.0f;

    private StatusComponent _statusComponent;
    public override void OnStart()
    {
        ranPos = InitPos.Value + Random.insideUnitSphere * _patrolRadius;

        _statusComponent = GetComponent<StatusComponent>();
        _statusComponent.IsCombat = false;
        
        IsPatrol.Value = true;
    }
    
    public override TaskStatus OnUpdate()
    {
        if (!NetworkServer.active) // 서버인지 확인
            return TaskStatus.Failure;
        
        //hit 된 위치가 NavMesh 상인지 체크하여 맞으면 이동 아니면 다시 재귀
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(ranPos, out navHit, _patrolRadius, NavMesh.AllAreas))
        {
            closestTarget.Value = navHit.position;
        }
        else
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }
}
