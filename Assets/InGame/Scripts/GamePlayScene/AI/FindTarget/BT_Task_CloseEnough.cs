using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BT_Task_CloseEnough : Action
{
    private float _distance = 5f;

    public SharedTransform closestTarget;
    public SharedTransformList SharedTransformList;

    public override void OnStart()
    {
        BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
        SharedTransformList = behaviorTree.GetVariable("TreeTargets") as SharedTransformList;
    }
    public override TaskStatus OnUpdate()
    {
        if (!NetworkServer.active) // 서버인지 확인
            return TaskStatus.Failure;
        
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
                isSuccess = true;

                closestTarget.Value = PlayerController.transform;
            }
        }

        return isSuccess ? TaskStatus.Success : TaskStatus.Failure;
    }
}
