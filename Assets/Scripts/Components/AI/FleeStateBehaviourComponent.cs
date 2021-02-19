using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeStateBehaviourComponent : IAiState
{
    [SerializeField] float distanceToFlee;
    public IAiState Execute(IAiStateMachine stateMachine, float deltaTime)
    {

        return this;
    }

    private void RunAway(Vector3 targetPosition)
    {

    }
}
