using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteeringBehaviour
{
    Vector3 CalculateSteeringForce(IMoverComponent mover, IFieldOfView fieldOfView);
}

public interface IFieldOfView
{
    float Radius { get; }
    float Angle { get; }
    List<GameObject> GameObjectsInView { get; }
    List<GameObject> GameObjectsInSurroundings { get; }
}

public interface IPlayerStateMachine
{
    //float ArrivingDistance { get; }
    PlayerStates CurrentState { get; set; }
    GameObject TargetObject { get; }
    Vector3 TargetLocation { get;}

    void SetTargetObject(GameObject gameObject);
    void SetTargetLocation(Vector3 target);


    //TODO
    //CharacterInfo
    //attackSpd
    //attackDamage
    //attackRange
    //movespeed
}

public interface IAiStateMachine
{
    IAiState CurrentState { get; }
    IDirectionMoverComponent MoverComponent { get; }
    IFieldOfView FieldOfViewComponent { get; }

    IAiState AttackState { get; }
    IAiState WanderState { get; }
    //IAiState IdleState { get; }
    IAiState EngagedState { get; }
    IAiState FleeState { get; }

    string[] EnemyTags { get; }
    string[] NeutralTags { get; }
    string[] FriendlyTags { get; }
}

public interface IAiState
{
    IAiState Execute(IAiStateMachine stateMachine, float deltaTime);
}

