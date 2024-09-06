using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class SetRandomPosition_Task : Action
{
    public Vector3 ranPos;
    public SharedVector3 SInitPos;
    
    //정찰위치
    public SharedVector3 SPatrolPos;
    
    private float _patrolRadius = 10.0f;
    
    public override void OnStart()
    {
        ranPos = SInitPos.Value + Random.insideUnitSphere * _patrolRadius;
    }
    
    public override TaskStatus OnUpdate()
    {
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(ranPos, out navHit, _patrolRadius, NavMesh.AllAreas))
        {
            SPatrolPos.Value = navHit.position;
        }
        else
        {
            return TaskStatus.Failure;
        }
        
        return TaskStatus.Success;
    }
}
