﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbTargetMoveSteeringComponent : MoverComponent, ITargetMoverComponent
{
    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } private set { _movement.DesiredPosition = value; } }

    //public bool hasArrived { get; private set; }

    //[SerializeField] float _maxSpeed = 1;
    [Header("RigidBody movement details:")]
    [SerializeField] bool ignoreYAxisDirection;
    [SerializeField] float distanceToArriveAtTargetLocation;
    [Space(10)]

    [Header("RigidBody rotation details:")]
    [SerializeField] bool rotateForwardToDirection;
    [SerializeField] bool lockRotationX;
    [SerializeField] bool lockRotationY;
    [SerializeField] bool lockRotationZ;

    Vector3 _direction;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = new Movement(_maxSpeed);
        _movement.DesiredPosition = _rb.position;
        _movement.LastDirectionFacing = _rb.transform.forward;
    }

    private void Update()
    {
        _direction += SteeringBehaviour.Seek(TargetPosition, _rb.velocity, this) * Time.deltaTime;
        _direction = SteeringBehaviour.Arriving(this, _direction, TargetPosition, distanceToArriveAtTargetLocation, 0.6f);
            //_movement.MoveTowards(_rb, TargetPosition, Time.deltaTime);

        _movement.AssignVelocity(_rb, Vector3.ClampMagnitude(_direction, _maxSpeed));

        if (rotateForwardToDirection)
        {
            //var rotation = Quaternion.identity;
            _rb.transform.LookAt(_movement.LastDirectionFacing + _rb.position, Vector3.up);
            var newRotation = _rb.transform.rotation;
            if (lockRotationX)
                newRotation.x = 0;
            if (lockRotationY)
                newRotation.y = 0;
            if (lockRotationZ)
                newRotation.z = 0;

            _rb.transform.rotation = newRotation;
        }

        Debug.DrawLine(_rb.transform.position, _movement.LastDirectionFacing + _rb.transform.position, Color.blue);

    }

    public void SetTargetPosition(Vector3 target)
    {

        if (ignoreYAxisDirection)
            target.y = target.y + _rb.position.y;

        TargetPosition = target;
        var direction = (target - _rb.position);


        _direction = direction;

        if (direction.magnitude != 0)
            _movement.LastDirectionFacing = _direction.normalized;
    }
}
