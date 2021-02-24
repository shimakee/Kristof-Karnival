using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IPlayerStateMachine), typeof(GetAngleBetweenCamCharFaceDirectionAnimatorComponent))]
public class AnimationHandlerComponent : MonoBehaviour
{

    [SerializeField] Animator animator;

    IPlayerStateMachine playerStateMachine;
    GetAngleBetweenCamCharFaceDirectionAnimatorComponent angleToCamComponent;

    private void Awake()
    {
        playerStateMachine = GetComponent<IPlayerStateMachine>();
        angleToCamComponent = GetComponent<GetAngleBetweenCamCharFaceDirectionAnimatorComponent>();
    }
    void LateUpdate()
    {
        animator.SetFloat("angleToCam", angleToCamComponent.AngledSigned);
        animator.SetInteger("State", (int)playerStateMachine.CurrentState);
    }
}

