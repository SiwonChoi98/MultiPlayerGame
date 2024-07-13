using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_Task_RotatePath : Action
{
    public SharedVector3 closestTarget;
    private RotateComponent _rotateComponent;
    
    public override void OnStart()
    {
        BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
        closestTarget = behaviorTree.GetVariable("TreeRandomPath") as SharedVector3;

        _rotateComponent = GetComponent<RotateComponent>();
    }

    public override TaskStatus OnUpdate()
    {
        // closestTarget의 위치를 가져와서 각도 계산
        Vector2 dirVec = closestTarget.Value - transform.position;
        float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;

        // RotateComponent를 사용하여 각도로 회전
        _rotateComponent.RotateTowardsMouse(angle);

        return TaskStatus.Success;
        
    }
}
