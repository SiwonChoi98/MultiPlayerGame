using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_Task_RotateTarget : Action
{
    public SharedTransform closestTarget;
    private RotateComponent _rotateComponent;
    
    public override void OnStart()
    {
        BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
        closestTarget = behaviorTree.GetVariable("TreeClosestTarget") as SharedTransform;

        _rotateComponent = GetComponent<RotateComponent>();
    }

    public override TaskStatus OnUpdate()
    {
        // closestTarget의 위치를 가져와서 각도 계산
        Vector2 dirVec = closestTarget.Value.position - transform.position;
        float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;

        // RotateComponent를 사용하여 각도로 회전
        _rotateComponent.RotateTowardsMouse(angle);

        return TaskStatus.Success;
        
    }
}
