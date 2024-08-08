using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class BT_Task_CloseEnough : Action
{
    //타겟 추적 거리
    private float _targetChaseDistance = 6f;

    public SharedTransform closestTarget;
    public SharedTransformList SharedTransformList;
    
    public override TaskStatus OnUpdate()
    {
        if (!NetworkServer.active) // 서버인지 확인
            return TaskStatus.Failure;
        
        float closestDistance = float.MaxValue;
        bool isSuccess = false;
        foreach (var PlayerController in SharedTransformList.Value)
        {
            if(PlayerController.GetComponent<StatusComponent>().IsDead)
                continue;
            
            Vector3 playerPosition = PlayerController.transform.position;
            Vector3 thisPosition = transform.position;

            float distance = Vector3.Distance(playerPosition, thisPosition);
            closestDistance = Mathf.Min(closestDistance, distance);
            if (distance <= _targetChaseDistance)
            {
                isSuccess = true;

                closestTarget.Value = PlayerController.transform;
            }
        }

        return isSuccess ? TaskStatus.Success : TaskStatus.Failure;
    }
}
