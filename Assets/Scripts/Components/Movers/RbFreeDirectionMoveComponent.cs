using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbFreeDirectionMoveComponent : MoverComponent, IDirectionMoverComponent, ITargetMoverComponent
{
    //[SerializeField] float _maxSpeed = 1;
    [SerializeField] bool ignoreYAxisDirection;
    [SerializeField] bool rotateForwardToDirection;

    public Vector3 Direction
    { 
        get { return _movement.Direction; } 
        private set 
        { 
            _movement.Direction = value; 
        } 
    }

    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } private set { _movement.DesiredPosition = value; } }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = new Movement(_maxSpeed);
        _movement.DesiredPosition = _rb.position;
        _movement.LastDirectionFacing = _rb.transform.forward;
    }

    private void Update()
    {
        _movement.AssignVelocity(_rb, Vector3.ClampMagnitude(Direction, _maxSpeed));

        if(rotateForwardToDirection)
            _rb.transform.LookAt(_movement.LastDirectionFacing + _rb.position, Vector3.up);

        Debug.DrawLine(_rb.transform.position, _movement.LastDirectionFacing + _rb.transform.position, Color.blue);
    }

    public void MoveDirection(Vector3 direction)
    {
        if (ignoreYAxisDirection)
            direction.y = 0;

        Direction = direction;

        if (direction.magnitude != 0)
            _movement.LastDirectionFacing = direction.normalized;
    }

    public void SetTargetPosition(Vector3 target)
    {
        TargetPosition = target;
        var direction = (target - _rb.position);

        if (ignoreYAxisDirection)
            direction.y = 0;

        Direction = direction;

        if (direction.magnitude != 0)
            _movement.LastDirectionFacing = Direction.normalized;
    }
}
