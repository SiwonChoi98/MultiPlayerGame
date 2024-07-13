using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Mirror;
using UnityEngine;

public class BT_Task_PathRandomSelect : Action
{
    public SharedVector3 closestTarget;
    public Vector3 ranPos ;
    
    public float Yrange;
    public float Xrange;
    
    public override void OnStart()
    {
        ranPos = transform.position +
                 new Vector3(Random.Range(-Xrange, Xrange), Random.Range(-Yrange, Yrange), 0);
    }
    
    public override TaskStatus OnUpdate()
    {
        if (!NetworkServer.active) // 서버인지 확인
            return TaskStatus.Failure;
        
        closestTarget.Value = ranPos;
        return TaskStatus.Success;
    }
}
