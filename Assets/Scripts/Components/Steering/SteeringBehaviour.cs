using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IForceMoverComponent))]
public class SteeringBehaviour : MonoBehaviour
{

    [SerializeField] GameObject target;
    [Range(0, 50)][SerializeField] float maxTravelSpeed;
    [Range(0, 50)] [SerializeField] float maxSteeringForce;

    IForceMoverComponent _mover;
    Rigidbody _rb;
    Vector3 _direction;
    private void Awake()
    {
        _mover = GetComponent<IForceMoverComponent>();
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        _direction = setDesiredVelocity(target.transform.position);
        //_mover.MoveDirection(direction);
    }

    private void FixedUpdate()
    {
        _mover.AddForce(_direction);
    }

    #region Seek
    private Vector3 setDesiredVelocity(Vector3 target)
        {
            var desiredVelocity = (target - _mover.CurrentPosition).normalized * maxTravelSpeed;
            var steering = Vector3.ClampMagnitude(desiredVelocity - _rb.velocity, maxSteeringForce);

            return steering;
        }
    #endregion
}
