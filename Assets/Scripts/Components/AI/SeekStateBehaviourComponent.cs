using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SeekStateBehaviourComponent : IAiState
{
    public Vector3 TargetPosition;
    public GameObject TargetObject;

    Vector3 _direction = Vector3.zero;

    public IAiState Execute(IAiStateMachine stateMachine, float deltaTime)
    {

        var enemies = CheckForEnemies(stateMachine);

        //go to position and arrive first if there are no enemies before returning to wander state.

        if (enemies == null)
            return stateMachine.WanderState;

        if (enemies.Length == 0)
            return stateMachine.WanderState;

        TargetObject = enemies[0];
        TargetPosition = TargetObject.transform.position;

        if(TargetObject != null)
        {
            _direction += SteeringBehaviour.Seek(TargetPosition, stateMachine.MoverComponent);

            //arrival behaviour
            stateMachine.MoverComponent.MoveDirection(_direction);
        }
        else
        {
            stateMachine.MoverComponent.MoveDirection(Vector3.zero);
        }

        return this;
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
