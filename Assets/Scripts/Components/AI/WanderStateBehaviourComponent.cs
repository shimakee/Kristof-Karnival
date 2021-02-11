
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WanderStateBehaviourComponent : IAiState
{
    [Range(-360, 360)] [SerializeField] public float maxDirectionAngleRange;
    [Range(.1f, 20f)] [SerializeField] float radius;
    [Range(0, 20f)] [SerializeField] float distanceAheadToCheck;
    [Range(0, 20f)] [SerializeField] float minDirectionTimeInterval;
    [Range(0, 20f)] [SerializeField] float maxDirectionTimeInterval;
    [Range(0, 30f)] [SerializeField] float minIdleTime;
    [Range(0, 30f)] [SerializeField] float maxIdleTime;


    private float _time;
    private float _idleTimeCounter;
    private Vector3 _direction;
    private Vector3 _wanderPoint;
    private float _wanderIntervalTime;
    private float _idleIntervalTime;

    public IAiState Execute(IAiStateMachine stateMachine, float deltaTime)
    {
        _time += deltaTime;

        _direction += Wander(stateMachine.MoverComponent, _wanderIntervalTime) * deltaTime;
        stateMachine.MoverComponent.MoveDirection(_direction);

        //check surroundings for enemy target or friendlies

        //implement obstacle avoidance


        return this;
    }

    #region Wander

    private Vector3 Wander(IMoverComponent mover, float interval)
    {
        if (_time > interval)
        {
            _time = 0;
            _wanderIntervalTime = UnityEngine.Random.Range(minDirectionTimeInterval, maxDirectionTimeInterval);
            _wanderPoint = PickAPointInFront(mover, distanceAheadToCheck, radius, maxDirectionAngleRange);
        }

        return SteeringBehaviour.Seek(_wanderPoint, mover);
    }

    private Vector3 PickAPointInFront(IMoverComponent mover, float distanceAhead, float radius, float angle)
    {
        var centerPoint = mover.CurrentPosition + (mover.LastDirectionFacing.normalized * distanceAhead);
        var destination = GetPointWithinACircle(centerPoint, radius, angle);

        return destination;
    }

    private Vector3 GetPointWithinACircle(Vector3 center, float radius, float maxAngle)
    {
        float randomAngle = UnityEngine.Random.Range(-360, maxAngle);

        float x = center.x + Mathf.Cos(randomAngle * Mathf.Deg2Rad) * radius;
        float z = center.z + Mathf.Sin(randomAngle * Mathf.Deg2Rad) * radius;

        return new Vector3(x, center.y, z);
    }
    #endregion
}
