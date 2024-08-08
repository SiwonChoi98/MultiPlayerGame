using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class BT_Task_ChasePath : Action
{
    public SharedVector3 closestPath;
    public SharedTransformList SharedTransformList;
    public Transform closestTarget;

    public SharedBool IsNotFindTarget;
    
    private float _notFindTargetTime;
    
    //타겟 추적 거리
    private float _targetChaseDistance = 6f;
    //이동 성공 범위
    private float _successRange = 0.2f; 
    
    private AgentMovement _agentMovement;
    
    public override void OnStart()
    {
        _agentMovement = GetComponent<AgentMovement>();
        _agentMovement.SetPlayPosition();
        
        _notFindTargetTime = 5f;
    }

    public override TaskStatus OnUpdate()
    {
        if (closestPath == null)
            return TaskStatus.Failure;
        
        _agentMovement.SetAgentPosition(closestPath.Value);

        float pathDistance = Vector2.Distance(transform.position, closestPath.Value);
        
        // 거리가 오차범위 이내이면 성공으로 취급
        if (pathDistance <= _successRange)
        {
            return TaskStatus.Success;
        }

        //타겟 찾아서 해당 거리안에 들어오면 실패 취급
        FindTarget();
        NotFindTargetTimeCalculate();
        if (closestTarget != null && !IsNotFindTarget.Value)
        {
            float distance = Vector3.Distance(closestTarget.position, transform.position);
        
            if (distance <= _targetChaseDistance)
            {
                return TaskStatus.Failure;
            }
        }
        
        return TaskStatus.Running;
    }

    private void FindTarget()
    {
        float closestDistance = float.MaxValue;
        foreach (var PlayerController in SharedTransformList.Value)
        {
            Vector3 playerPosition = PlayerController.transform.position;
            Vector3 thisPosition = transform.position;

            float distance = Vector3.Distance(playerPosition, thisPosition);
            closestDistance = Mathf.Min(closestDistance, distance);
            if (distance <= _targetChaseDistance)
            {
                closestTarget = PlayerController.transform;
            }
        }
    }

    private void NotFindTargetTimeCalculate()
    {
        if (IsNotFindTarget.Value)
        {
            _notFindTargetTime -= Time.deltaTime;
            if (_notFindTargetTime < 0)
            {
                IsNotFindTarget.Value = false;
            }
        }
    }
}
