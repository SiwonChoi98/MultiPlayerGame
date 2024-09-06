using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;

public class CheckTarget_Task : Action
{
    /// <summary>
    /// 범위안에 타겟이 있는지 체크하여 타겟 지정
    /// </summary>
    
    //타겟
    public SharedTransform Target;
    
    //타겟을 찾을 범위 
    public SharedFloat SFindTargetRadius;
    
    //찾을 타겟 레이어마스크
    public SharedLayerMask SFindTargetLayerMask;
    
    public override TaskStatus OnUpdate()
    {
        SetNearTarget();

        if (Target.Value != null)
        {
            ReleaseTarget();
        }
        
        return TaskStatus.Success;
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
                Target.Value = hitCollider.transform;
                Debug.Log(hitCollider.transform.GetComponent<InGameUserInfo>().UserName);
            }
        }
    }

    private void ReleaseTarget()
    {
        Vector3 targetPosition = Target.Value.transform.position;
        float currentDistance = DistanceCalculate(targetPosition);
        
        if (currentDistance > SFindTargetRadius.Value)
        {
            Target.Value = null;
        }
    }

    private float DistanceCalculate(Vector3 targetPos)
    {
        Vector3 thisPosition = transform.position;
        float currentDistance = Vector3.Distance(targetPos, thisPosition);
        
        return currentDistance;
    } 
    
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawSphere(transform.position, SFindTargetRadius.Value);
    }

}
