using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateMachine : MonoBehaviour, IPlayerStateMachine
{
    public Animator Animator;
    public PlayerStates CurrentState { get; set; }
    public GameObject TargetObject { get; private set; }
    public Vector3 TargetLocation { get; private set; }

    //public float ArrivingDistance { get; set; }

    float _distance;

    public void SetTargetLocation(Vector3 target)
    {
        TargetLocation = target;

        _distance = Vector3.Distance(TargetLocation, transform.position);

        if (_distance >= .06f)
            this.CurrentState = PlayerStates.move;
        else
            this.CurrentState = PlayerStates.idle;

        Animator.SetInteger("State", (int)this.CurrentState);
    }

    public void SetTargetObject(GameObject gameObject)
    {

        if (gameObject == null)
            throw new NullReferenceException("game object cannot be null.");

        TargetObject = gameObject;
    }

    private void Update()
    {


        //if (_distance < arrivingDistance)
        //{
        //    switch (_playerStateMachine.TargetObject.tag)
        //    {

        //        case "Ground":
        //            animator.SetInteger("State", (int)PlayerStates.idle);
        //            break;
        //        case "Enemy":
        //            animator.SetFloat("State", (int)PlayerStates.attack);
        //            break;
        //        //case "Friendly":
        //        //    Animator.SetFloat("State", (int)PlayerStates);
        //        //    break;
        //        default:
        //            animator.SetFloat("State", (int)PlayerStates.idle);
        //            break;
        //    }
        //}


    }
}

public enum PlayerStates
{
    idle,
    move,
    attack
}
