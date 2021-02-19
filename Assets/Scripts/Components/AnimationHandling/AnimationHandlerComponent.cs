using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMoverComponent), typeof(GetAngleBetweenCamCharFaceDirectionAnimatorComponent))]
public class AnimationHandlerComponent : MonoBehaviour
{

    [SerializeField] Animator animator;

    IMoverComponent mover;
    GetAngleBetweenCamCharFaceDirectionAnimatorComponent angleToCamComponent;

    private void Awake()
    {
        mover = GetComponent<IMoverComponent>();
        angleToCamComponent = GetComponent<GetAngleBetweenCamCharFaceDirectionAnimatorComponent>();
    }
    void LateUpdate()
    {
        animator.SetFloat("Magnitude", mover.CurrentVelocity.magnitude);
        animator.SetFloat("angleToCam", angleToCamComponent.AngledSigned);
    }
}

