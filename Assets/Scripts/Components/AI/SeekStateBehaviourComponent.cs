using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SeekStateBehaviourComponent : IAiState
{
    [SerializeField] float attackDistance;
    [SerializeField] float attackInterval;
    [SerializeField] float minDistanceToMaintain;
    [SerializeField] float maxDistanceToMaintain;

    private Vector3 TargetPosition;
    private GameObject TargetObject;
    private float _attackTimer;

    Vector3 _direction = Vector3.zero;

    public IAiState Execute(IAiStateMachine stateMachine, float deltaTime)
    {
        _attackTimer += deltaTime;

        if (TargetObject == null)
        {
            var enemies = CheckForEnemies(stateMachine);

            //go to position and arrive first if there are no enemies before returning to wander state.

            if (enemies == null)
                return stateMachine.WanderState;

            if (enemies.Length == 0)
                return stateMachine.WanderState;

            TargetObject = enemies[0];
            Reposition(TargetObject, stateMachine, deltaTime);
        }
        else
        {
            Reposition(TargetObject, stateMachine, deltaTime);
        }

        stateMachine.MoverComponent.MoveDirection(_direction);


        return this;
    }

    private void Reposition(GameObject targetObject, IAiStateMachine stateMachine, float deltaTime)
    {
        
        var targetPosition = targetObject.transform.position;
        float distance = Vector3.Distance(targetPosition, stateMachine.MoverComponent.CurrentPosition);

        bool isInAttackRange = distance <= attackDistance;
        bool canAttack = _attackTimer > attackInterval;
        bool isClose = distance < minDistanceToMaintain;
        bool isFar = distance > maxDistanceToMaintain;
        bool isTooFar = distance > stateMachine.FieldOfViewComponent.Radius;

        if (isTooFar)
            TargetObject = null;

        //if low on life go to flee state

        if(canAttack && isInAttackRange)
        {
            //go to attack state
            Attack(TargetObject);
        }

        if (isClose)
            _direction += SteeringBehaviour.Flee(targetPosition, _direction, stateMachine.MoverComponent) * deltaTime;
        else if (isFar)
            _direction += SteeringBehaviour.Seek(targetPosition, _direction, stateMachine.MoverComponent) * deltaTime;
        else
        {
            var direction = SteeringBehaviour.Seek(targetPosition, _direction, stateMachine.MoverComponent).normalized;
            stateMachine.MoverComponent.MoveDirection(direction);
            _direction = SteeringBehaviour.Arriving(stateMachine.MoverComponent, _direction, targetPosition, maxDistanceToMaintain, .05f);
        }
        //arrival behaviour
        //attack
        //roam around

    }

    private void Attack(GameObject target)
    {
        _attackTimer = 0;Debug.Log($"attacked {target.name}");
    }

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
}
