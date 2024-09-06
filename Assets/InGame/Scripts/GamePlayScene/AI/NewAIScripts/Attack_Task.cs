using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Attack_Task : Action
{
    /// <summary>
    /// 공격
    /// </summary>
    
    //공격 컴포넌트
    private CombatComponent _combatComponent;
    //이동 컴포넌트
    private AgentMovement _agentMovement;
    //타겟
    public SharedTransform STarget;
    
    public override void OnStart()
    {
        _combatComponent = GetComponent<CombatComponent>();
        _agentMovement = GetComponent<AgentMovement>();
    }

    public override TaskStatus OnUpdate()
    {
        if (STarget.Value == null)
            return TaskStatus.Failure;
        
        _agentMovement.SetStopPosition(STarget.Value.position);
        
        Attack();
        
        return TaskStatus.Success;
    }

    private void Attack()
    {
        _combatComponent.Server_HitCheck(4f);
    }
}
