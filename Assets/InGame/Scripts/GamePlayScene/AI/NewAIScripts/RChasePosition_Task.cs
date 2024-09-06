using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class RChasePosition_Task : Action
{
    //해당 위치로 이동하면서 타겟 체크
    private AgentMovement _agentMovement;
    
    //지정위치
    public SharedVector3 SPatrolPos;
    
    //공격받은 위치
    public SharedVector3 STakeDamagedPos;
    
    //타겟
    public SharedTransform STarget;
    
    //타겟을 찾을 범위 
    public SharedFloat SFindTargetRadius;
    
    //찾을 타겟 레이어마스크
    public SharedLayerMask SFindTargetLayerMask;
    
    //이동 성공 범위
    private float _successRange = 0.2f;

    public override void OnStart()
    {
        _agentMovement = GetComponent<AgentMovement>();
    }

    public override TaskStatus OnUpdate()
    {
        _agentMovement.SetAgentPosition(SPatrolPos.Value);

        //공격받은 위치가 있다면 faild
        CheckTakeDamaged();
        if (STakeDamagedPos.Value != Vector3.zero)
        {
            return TaskStatus.Failure;
        }

        //타겟이 있으면 faild
        SetNearTarget();
        if (STarget.Value != null)
        {
            return TaskStatus.Failure;
        }
        
        float pathDistance = Vector2.Distance(transform.position, SPatrolPos.Value);
        
        // 거리가 오차범위 이내이면 성공으로 취급
        if (pathDistance <= _successRange)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
    
    private void CheckTakeDamaged()
    {
        StatusComponent statusComponent = GetComponent<StatusComponent>();
        
        if (statusComponent.IsAIDamaged)
        {
            STakeDamagedPos.Value = statusComponent.ShotPlayerPos;
            statusComponent.IsAIDamaged = false;
        }
    }
    
    private void SetNearTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, SFindTargetRadius.Value, SFindTargetLayerMask.Value);
        
        float targetDistance = float.MaxValue;
        foreach (var hitCollider in colliders)
        {
            if (hitCollider.transform.GetComponent<StatusComponent>().IsDead)
                continue;
            
            Vector3 hitPosition = hitCollider.transform.position;
            
            float currentDistance = DistanceCalculate(hitPosition);
            targetDistance = Mathf.Min(targetDistance, currentDistance);
            
            if (targetDistance <= SFindTargetRadius.Value)
            {
                STarget.Value = hitCollider.transform;
                Debug.Log(hitCollider.transform.GetComponent<InGameUserInfo>().UserName);
            }
        }
    }
    
    private float DistanceCalculate(Vector3 targetPos)
    {
        Vector3 thisPosition = transform.position;
        float currentDistance = Vector3.Distance(targetPos, thisPosition);
        
        return currentDistance;
    } 
}
