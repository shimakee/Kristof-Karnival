using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RbFreeMove2DComponent : MonoBehaviour, IMoverComponent
{
    [SerializeField] float speed = 1;
    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } set { _movement.DesiredPosition = value; } }
    public Vector3 Direction { get { return _movement.Direction; } set { _movement.Direction = value; } }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }


    Movement _movement;
    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = new Movement(speed);
        _movement.DesiredPosition = _rb.position;
    }

    private void FixedUpdate()
    {
        _movement.AssignVelocity2D(_rb, Direction);
    }


    public void Move(Vector3 direction)
    {
        Direction = direction;
    }
}
