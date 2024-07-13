using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;

public class BT_Task_ChasePath : Action
{
    public SharedVector3 closestPath;
    public SharedTransformList SharedTransformList;
    public Transform closestTarget;
    
    private float _distance = 5f;
    
    private float _successRange = 0.2f; //성공 범위
    private MoveComponent _moveComponent;
    
    
    public override void OnStart()
    {
        BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
        closestPath = behaviorTree.GetVariable("TreeRandomPath") as SharedVector3;
        SharedTransformList = behaviorTree.GetVariable("TreeTargets") as SharedTransformList;
        
        _moveComponent = GetComponent<MoveComponent>();
    }

    public override TaskStatus OnUpdate()
    {
        if (closestPath == null)
            return TaskStatus.Failure;

        
        Vector2 dirVec = closestPath.Value - transform.position;
        _moveComponent.Server_Move(dirVec);
        

        float pathDistance = Vector2.Distance(transform.position, closestPath.Value);
        
        // 거리가 오차범위 이내이면 성공으로 취급
        if (pathDistance <= _successRange)
        {
            return TaskStatus.Success;
        }

        //타겟 찾아서 해당 거리안에 들어오면 실패 취급
        FindTarget();
        if (closestTarget != null)
        {
            float distance = Vector3.Distance(closestTarget.position, transform.position);
        
            if (distance <= _distance)
            {
                return TaskStatus.Failure;
            }
        }
        return TaskStatus.Running;
    }

    private void FindTarget()
    {
        float closestDistance = float.MaxValue;
        bool isSuccess = false;
        foreach (var PlayerController in SharedTransformList.Value)
        {
            Vector3 playerPosition = PlayerController.transform.position;
            Vector3 thisPosition = transform.position;

            float distance = Vector3.Distance(playerPosition, thisPosition);
            closestDistance = Mathf.Min(closestDistance, distance);
            if (distance <= _distance)
            {
                closestTarget = PlayerController.transform;
            }
        }
    }
}
