using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent))]
public class SteeringBehaviour : MonoBehaviour
{

    [SerializeField] GameObject target;
    [SerializeField] float maxSpeed;

    IDirectionMoverComponent _mover;
    Rigidbody _rb;
    private void Awake()
    {
        _mover = GetComponent<IDirectionMoverComponent>();
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        //_mover.MoveDirection(setDesiredVelocity(target.transform.position));
        _rb.AddForce(setDesiredVelocity(target.transform.position));
        //_mover.MoveDirection(setDesiredVelocity(target.transform.position));
    }

    private Vector3 setDesiredVelocity(Vector3 target)
    {
        var desiredVelocity = (target - _rb.position).normalized * maxSpeed;
        var adjustedVelocity = Vector3.ClampMagnitude(desiredVelocity - _rb.velocity, maxSpeed);

        return adjustedVelocity;
    }
}
