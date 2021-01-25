using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbFreeDirectionMoveComponent : MonoBehaviour, IDirectionMoverComponent, ITargetMoverComponent
{
    [SerializeField] float maxSpeed = 1;

    public Vector3 Direction
    { 
        get { return _movement.Direction; } 
        private set 
        { 
            _movement.Direction = value; 
        } 
    }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }

    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } private set { _movement.DesiredPosition = value; } }

    Movement _movement;
    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = new Movement(maxSpeed);
        _movement.DesiredPosition = _rb.position;
        _movement.LastDirectionFacing = _rb.transform.forward;
    }

    private void FixedUpdate()
    {
        _movement.AssignVelocity(_rb, Vector3.ClampMagnitude(Direction, maxSpeed));
        _movement.RotateForwardDirectionInYAxis(_rb, _movement.LastDirectionFacing);

        Debug.DrawLine(_rb.transform.position, _movement.LastDirectionFacing + _rb.transform.position, Color.blue);
    }

    public void MoveDirection(Vector3 direction)
    {
        Direction = direction;

        if (direction.magnitude != 0)
            _movement.LastDirectionFacing = direction.normalized;
    }

    public void SetTargetPosition(Vector3 target)
    {
        TargetPosition = target;

        Direction = (target - _rb.position);
    }
}
