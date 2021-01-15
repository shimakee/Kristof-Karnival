﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMoverComponent))]
public class GetAngleBetweenCamCharFaceDirectionAnimatorComponent : MonoBehaviour
{
    [SerializeField] Camera cameraToGetAngle;
    [SerializeField] Animator animator;
    [SerializeField] string animatorParameterName;
    [SerializeField] bool inputAsReferenceDirection;
    [SerializeField] bool ignoreYAxis;

    IMoverComponent _mover;
    Vector3 _directionFacing;
    private void Awake()
    {
        _mover = GetComponent<IMoverComponent>();
    }

    private void LateUpdate()
    {
        if (inputAsReferenceDirection)
            _directionFacing = _mover.LastDirectionFacing;
        else
            _directionFacing = transform.forward;

        var currentPosition = transform.position;
        var directionToCam = cameraToGetAngle.transform.position - currentPosition;

        if (ignoreYAxis)
        {
            _directionFacing.y = 0;
            directionToCam.y = 0;
        }

        var angleSigned = Vector3.SignedAngle(directionToCam, _directionFacing, currentPosition);
        Debug.Log($"cam angle {angleSigned}");


        animator.SetFloat(animatorParameterName, angleSigned);
    }
}
