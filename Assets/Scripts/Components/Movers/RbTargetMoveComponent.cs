using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbTargetMoveComponent : MonoBehaviour, ITargetMoverComponent
{
    [SerializeField] float speed = 1;

    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } set { _movement.DesiredPosition = value; } }
    public Vector3 Direction { get { return _movement.Direction; } set { _movement.Direction = value; if (value.magnitude != 0) _movement.LastDirectionFacing = value.normalized; } }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }


    Movement _movement;
    Rigidbody _rb;

    private void Awake()
    {
        _movement = new Movement(speed);
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _movement.MoveTowards(_rb, _movement.DesiredPosition, Time.fixedDeltaTime);
    }
}
