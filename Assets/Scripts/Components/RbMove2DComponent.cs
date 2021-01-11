using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RbMove2DComponent : MonoBehaviour, IMoverComponent
{
    [SerializeField] float speed = 1;

    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 Direction { get { return _movement.Direction; } set { _movement.Direction = value; } }
    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } set { _movement.DesiredPosition = value; } }

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
        //_movement.MoveTowards2D(_rb, TargetPosition, Time.fixedDeltaTime);
        _movement.AssignVelocity2D(_rb, Direction);
    }
}

public interface IMoverComponent
{
    Vector3 TargetPosition { get; set; }
    Vector3 Direction { get; set; }
    Vector3 CurrentPosition { get; }
}