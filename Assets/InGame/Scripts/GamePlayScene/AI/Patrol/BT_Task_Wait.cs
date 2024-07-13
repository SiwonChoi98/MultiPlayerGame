using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BT_Task_Wait : Action
{
    private float _distance = 5f;
    public float _waitTime = 5f;
    
    public Transform closestTarget;
    public SharedTransformList SharedTransformList;
    
    public override void OnStart()
    {
        _waitTime = 5f;
        
        BehaviorTree behaviorTree = GetComponent<BehaviorTree>();
        SharedTransformList = behaviorTree.GetVariable("TreeTargets") as SharedTransformList;
    }
    public override TaskStatus OnUpdate()
    {
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
        
        //WaitTime 기다리고 시간 지나면 성공 취급
        _waitTime -= Time.deltaTime;
        if (_waitTime < 0)
        {
            return TaskStatus.Success;
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
