using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class RCheckAndWait_Task : Action
{
    //대기하면서 타겟이 있는지 체크
    
    //공격받은 위치
    public SharedVector3 STakeDamagedPos;

    //대기 시간
    public float WaitTime = 4f;
    
    //타겟
    public SharedTransform STarget;
    
    //타겟을 찾을 범위 
    public SharedFloat SFindTargetRadius;
    
    //찾을 타겟 레이어마스크
    public SharedLayerMask SFindTargetLayerMask;
    
    public override void OnStart()
    {
        WaitTime = 5f;
    }

    public override TaskStatus OnUpdate()
    {
        WaitTime -= Time.deltaTime;
        if (WaitTime < 0)
        {
            return TaskStatus.Success;
        }
        
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
