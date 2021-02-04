using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAnimatorBehaviourScript : StateMachineBehaviour
{
    public float speed;
    public float steering;
    private ISteeringBehaviour seek;
    private IDirectionMoverComponent mover;
    private IFieldOfView fieldOfView;
    Vector3 _direction = Vector3.zero;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mover = animator.gameObject.GetComponent<IDirectionMoverComponent>();
        seek = new SeekBehaviour(speed, steering);
        fieldOfView = animator.gameObject.GetComponent<IFieldOfView>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _direction += seek.CalculateSteeringForce(mover, fieldOfView) * Time.deltaTime;
        mover.MoveDirection(_direction);
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
