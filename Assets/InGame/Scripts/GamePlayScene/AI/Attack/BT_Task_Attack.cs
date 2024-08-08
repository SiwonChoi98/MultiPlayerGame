using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;

public class BT_Task_Attack : Action
{
    private CombatComponent _combatComponent;
    [SerializeField] private AudioClip _audioClip;

    public SharedBool IsNotFindTarget = false;
    public SharedBool IsPatrol;
    public override void OnStart()
    {
        _combatComponent = GetComponent<CombatComponent>();
        IsNotFindTarget.Value = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (!NetworkServer.active) // 서버인지 확인
            return TaskStatus.Failure;
        
        return HitCheck();
    }
    
    private TaskStatus HitCheck()
    {
        if (IsPatrol.Value == true)
            return TaskStatus.Failure;
        
        _combatComponent.Server_HitCheck();
        
        IsPatrol.Value = false;
        if (_combatComponent.IsWall)
        {
            IsNotFindTarget.Value = true;
        }
        
        return TaskStatus.Failure;
    }
}
