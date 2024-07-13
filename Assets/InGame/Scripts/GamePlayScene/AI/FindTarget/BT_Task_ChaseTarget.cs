using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;

public class BT_Task_ChaseTarget : Action
{
    public SharedTransform closestTarget;
    private MoveComponent _moveComponent;
    
    public override void OnStart()
    {
        BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
        closestTarget = behaviorTree.GetVariable("TreeClosestTarget") as SharedTransform;

        _moveComponent = GetComponent<MoveComponent>();
    }

    public override TaskStatus OnUpdate()
    {
        Vector2 dirVec = closestTarget.Value.position - transform.position;
        _moveComponent.Server_Move(dirVec);

        return TaskStatus.Success;
    }
}
