using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent), typeof(IFieldOfView))]
public class AIStateMachineComponent : MonoBehaviour, IAiStateMachine
{
    public IAiState CurrentState { get { return _currentState; } }
    public IDirectionMoverComponent MoverComponent { get { return _mover; } }
    public IFieldOfView FieldOfViewComponent { get { return _fieldOfView; } }
    public IAiState AttackState { get { return _attackState; } }
    public IAiState WanderState { get { return wanderState; } }
    public IAiState EngagedState { get { return engagedState; } }
    public IAiState FleeState { get { return _fleeState; } }

    public string[] EnemyTags { get { return enemyTags; } }
    public string[] NeutralTags { get { return neutralTags; } }
    public string[] FriendlyTags { get { return friendlyTags; } }

    private IAiState _currentState;
    private IDirectionMoverComponent _mover;
    private IFieldOfView _fieldOfView;

    [SerializeField] private IAiState _attackState;
    public WanderStateBehaviourComponent wanderState;
    public SeekStateBehaviourComponent engagedState;
    [SerializeField] private IAiState _fleeState;

    [SerializeField] private string[] enemyTags;
    [SerializeField] private string[] neutralTags;
    [SerializeField] private string[] friendlyTags;

    private void Awake()
    {
        _mover = GetComponent<IDirectionMoverComponent>();
        _fieldOfView = GetComponent<IFieldOfView>();
        _currentState = wanderState;
    }

    // Update is called once per frame
    void Update()
    {
        _currentState = _currentState.Execute(this, Time.deltaTime);
    }
}

