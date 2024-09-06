using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ChaseFindPoint_Task : Action
{
    //타겟이 null이 될때 위치를 받아서 해당 위치로 이동
    
    //위치도착시 마지막 위치 null? 이런식으로 해서 feild 해줘야함\
    
    //타겟
    public SharedTransform STarget;
    //이동 컴포넌트
    private AgentMovement _agentMovement;
    //공격받은 위치
    public SharedVector3 STakeDamagedPos;

    public override void OnStart()
    {
        _agentMovement = GetComponent<AgentMovement>();
    }
    
    public override TaskStatus OnUpdate()
    {
        if (STarget.Value == null && STakeDamagedPos.Value != Vector3.zero)
        {
            //이동
            _agentMovement.SetAgentPosition(STakeDamagedPos.Value);
            
            return CheckEndPos();
        }

        
        return TaskStatus.Failure;
    }

    //해당 거리로 이동이 됐으면 fail 처리
    private TaskStatus CheckEndPos()
    {
        Vector3 thisPosition = transform.position;
        float currentDistance = Vector3.Distance(STakeDamagedPos.Value, thisPosition);

        if (currentDistance <= 1f)
        {
            STakeDamagedPos.Value = Vector3.zero;
            return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}
