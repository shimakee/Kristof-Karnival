
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WanderStateBehaviourComponent : IAiState
{
    [Range(-360, 360)] [SerializeField] public float maxDirectionAngleRange;
    [Range(.1f, 20f)] [SerializeField] float radius;
    [Range(0, 20f)] [SerializeField] float distanceAheadToCheck;
    [Range(0, 20f)] [SerializeField] float minDirectionTimeInterval;
    [Range(0, 20f)] [SerializeField] float maxDirectionTimeInterval;
    [Range(1, 10)] [SerializeField] int numberOfDirectionChanges;


    private float _time;
    private Vector3 _direction;
    private Vector3 _wanderPoint;
    private float _wanderIntervalTime;
    private int _directionChangeCount;

    public IAiState Execute(IAiStateMachine stateMachine, float deltaTime)
    {
        _time += deltaTime;


        var enemies = CheckForEnemies(stateMachine);

        if (enemies != null)
        {
            if(enemies.Length > 0)
                return stateMachine.EngagedState;
        }

        //Wandering
        //if (_directionChangeCount < numberOfDirectionChanges)
        //{
        _direction += Wander(stateMachine.MoverComponent, _wanderIntervalTime) * deltaTime;
        stateMachine.MoverComponent.MoveDirection(_direction);

            //return this;
        //}
        //else
        //{
        //    //_directionChangeCount = 0;
        //    //return idle state
        //}

        return this;
    }

    #region Wander

    private Vector3 Wander(IMoverComponent mover, float interval)
    {
        if (_time > interval)
        {
            _time = 0;
            _directionChangeCount++;

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
