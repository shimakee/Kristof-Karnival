
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WanderStateBehaviourComponent : IAiState
{
    [Header("Wandering details:")]
    [Range(-360, 360)] [SerializeField] public float maxDirectionAngleRange;
    [Range(.1f, 20f)] [SerializeField] float radius;
    //[Range(0, 20f)] [SerializeField] float distanceAheadToCheck;
    [Space(10)]


    [Header("Wander time info:")]
    [Range(0, 20f)] [SerializeField] float minWanderTimeInterval;
    [Range(0, 20f)] [SerializeField] float maxWanderTimeInterval;
    [Range(0, 20f)] [SerializeField] float minDistanceAhead;
    [Range(0, 20f)] [SerializeField] float maxDistanceAhead;
    [Space(10)]

    [Header("Arriving behaviour details:")]
    [Range(0, 20f)] [SerializeField] float distancePointToStop;
    [Range(0, 1)] [SerializeField] float thresholdToZeroVelocity;
    [Space(10)]

    [Header("SpawnPoin details:")]
    [SerializeField] Vector3 spawnPoint;
    [Range(0, 2)] [SerializeField] float stayNearWeight;
    [Range(0, 20)] [SerializeField] float minDistanceFromSpawnPoint;
    [Range(0, 20)] [SerializeField] float maxDistanceFromSpawnPoint;



    private float _time;
    private Vector3 _direction;
    private Vector3 _wanderPoint;
    private float _wanderIntervalTime;
    private float _distanceAhead;
    private float _distanceFromSpawnPoint;
    //private float _idleIntervalTime;
    //private float _idleTime;
    //private int _directionChangeCount;
    //private int _directionChangeInterval;

    public IAiState Execute(IAiStateMachine stateMachine, float deltaTime)
    {
        _time += deltaTime;
        _distanceFromSpawnPoint = Vector3.Distance(spawnPoint, stateMachine.MoverComponent.CurrentPosition);


        var enemies = CheckForEnemies(stateMachine);

        if (enemies != null)
        {
            if(enemies.Length > 0)
                return stateMachine.EngagedState;
        }

        _direction += Wander(stateMachine.MoverComponent, _wanderIntervalTime) * deltaTime * ComputeWanderWeight(1);
        _direction += SteeringBehaviour.Seek(spawnPoint,_direction, stateMachine.MoverComponent) * deltaTime * ComputeSeekSpawnPointWeight(stayNearWeight);
        _direction = SteeringBehaviour.Arriving(stateMachine.MoverComponent, 
                                                _direction, 
                                                _wanderPoint, 
                                                distancePointToStop, 
                                                thresholdToZeroVelocity);

        stateMachine.MoverComponent.MoveDirection(_direction);

        return this;
    }

    #region Weighting

    private float ComputeSeekSpawnPointWeight(float weight)
    {
        float seekWeight = SteeringBehaviour.ComputeWeightByDistanceFurtherHigher(_distanceFromSpawnPoint, weight, maxDistanceFromSpawnPoint, minDistanceFromSpawnPoint);
        //float seekWeight = weight * multiplier;
        Debug.Log($"seek weight {seekWeight}");
        return seekWeight;
    }

    private float ComputeWanderWeight(float weight)
    {
        float wanderWeight = SteeringBehaviour.ComputeWeightByDistanceFurtherLower(_distanceFromSpawnPoint, weight,maxDistanceFromSpawnPoint, minDistanceFromSpawnPoint);
        //float wanderWeight = weight * multiplier;
        Debug.Log($"wander weight {wanderWeight}");
        return wanderWeight;
    }

    #endregion

    #region Wander

    private Vector3 Wander(IMoverComponent mover, float interval)
    {
        if (_time > interval)
        {
            _time = 0;
            //_directionChangeCount++;

            Debug.Log($"wandering");
            //Debug.Log($"change direction {_directionChangeCount}");

            _wanderIntervalTime = UnityEngine.Random.Range(minWanderTimeInterval, maxWanderTimeInterval);
            _distanceAhead = UnityEngine.Random.Range(minDistanceAhead, maxDistanceAhead);
            _wanderPoint = PickAPointInFront(mover, _distanceAhead, radius, maxDirectionAngleRange);
        }

        return SteeringBehaviour.Seek(_wanderPoint, _direction, mover);
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

    #region Idle

    //private Vector3 Idle(float deltaTime)
    //{
    //    _idleTime += deltaTime;

    //    if (_idleTime > _idleIntervalTime)
    //    {
    //        Debug.Log("idle done");

    //        _directionChangeCount = 0;
    //        _idleTime = 0;

    //        _idleIntervalTime = UnityEngine.Random.Range(minWanderTimeInterval, maxWanderTimeInterval);
    //        _directionChangeInterval = UnityEngine.Random.Range(minNumberOfDirectionChanges, maxNumberOfDirectionChanges);

    //        //return _direction;
    //    }

    //    return Vector3.zero;
    //}

    #endregion

    #region Check For Enemies

    private GameObject[] CheckForEnemies(IAiStateMachine stateMachine)
    {
        //visibility Check
        var objectsInView = stateMachine.FieldOfViewComponent.GameObjectsInView;
        //check surroundings for enemy target or friendlies
        if (stateMachine.EnemyTags.Length > 0 && objectsInView.Count > 0)
        {

            string[] tags = stateMachine.EnemyTags;
            GameObject[] enemies = objectsInView.Where(item => tags.Contains(item.tag)).ToArray();

            //first enemy
            //closest enemy
            //lowest health enemy
            return enemies;
        }
        //if enemy - change to engaged behaviour
        //if friendly - implement cohesion behaviour?
        //if neutral - do nothing

        //implement obstacle avoidance
        return null;
    }
    #endregion
}
