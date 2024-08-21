using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    private RotateComponent _rotateComponent;
    private NavMeshAgent _navMeshAgent;
   
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rotateComponent = GetComponent<RotateComponent>();
        
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }
    
    public void SetAgentPosition(Vector3 target)
    {
        if (!gameObject.activeSelf)
            return;
        
        Vector2 dirVec = target - transform.position;
        float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
        
        _rotateComponent.RotateTowardsMouse(angle);
        _navMeshAgent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }

    public void SetStopPosition()
    {
        _navMeshAgent.isStopped = true;
    }

    public void SetPlayPosition()
    {
        _navMeshAgent.isStopped = false;
    }
}
