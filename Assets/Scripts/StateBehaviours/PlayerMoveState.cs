using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(IDirectionMoverComponent))]
public class PlayerMoveState : StateMachineBehaviour
{
    [SerializeField] float arrivingDistance;
    [SerializeField] float distanceThreshold = .6f;
    [SerializeField] float attackDistance;

    IPlayerStateMachine _playerStateMachine;
    IDirectionMoverComponent _mover;

    float _distance;
    Vector3 _direction;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerStateMachine = animator.GetComponent<IPlayerStateMachine>();
        _mover = animator.GetComponent<IDirectionMoverComponent>();
        //_mover.SetTargetPosition(_playerStateMachine.TargetLocation);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distance = Vector3.Distance(_playerStateMachine.TargetLocation, _mover.CurrentPosition);

        if (_playerStateMachine.TargetObject.tag == "Enemy")
            _playerStateMachine.SetTargetLocation(_playerStateMachine.TargetObject.transform.position);


            _direction += SteeringBehaviour.Seek(_playerStateMachine.TargetLocation, _mover.CurrentVelocity, _mover) * Time.deltaTime;
            _direction = SteeringBehaviour.Arriving(_mover, _direction, _playerStateMachine.TargetLocation, arrivingDistance, distanceThreshold);


        if (_playerStateMachine.TargetObject.tag == "Enemy" && _distance <= attackDistance)
        {
            _direction = Vector3.zero;
            _playerStateMachine.CurrentState = PlayerStates.attack;
        }

        if (_distance <= arrivingDistance + distanceThreshold)
        {
            _direction = Vector3.zero;
            _playerStateMachine.CurrentState = PlayerStates.idle;
        }

        _mover.MoveDirection(_direction);
        animator.SetInteger("State", (int)_playerStateMachine.CurrentState);

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
