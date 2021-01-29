using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RbGridMove2DComponent : MoverComponent, IMoverComponent
{
    [SerializeField] float speed = 1;
    [SerializeField] float holdTime = .2f;
    [SerializeField] float transitionTime = .1f;
    [SerializeField] ConnectedGridMap gridMap;

    //public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 Direction { get { return _movement.Direction; } set { _movement.Direction = value; } }
    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } set { _movement.DesiredPosition = value; } }
    //public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }


    //Movement _movement;
    new Rigidbody2D _rb;

    float _movementInputPressedTime = 0;

    private void Awake()
    {
        _movement = new Movement(speed);
        _rb = GetComponent<Rigidbody2D>();
        _movement.DesiredPosition = _rb.position;
    }

    private void FixedUpdate()
    {
        //snap to nearest tile, goes back if not enough
        //TargetPosition = gridMap.GetNearestTilePosition(_rb.position + (Vector2)Direction);

        if (Direction.magnitude > 0)
            _movementInputPressedTime += Time.deltaTime;
        else
            _movementInputPressedTime = 0;

        if (_movementInputPressedTime >= holdTime)
            if (_rb.position == (Vector2)TargetPosition)
            {
                _movementInputPressedTime = transitionTime;
                TargetPosition = gridMap.GetNearestTilePosition( TargetPosition + Direction);
            }

        _movement.MoveTowards2D(_rb, TargetPosition, Time.fixedDeltaTime * speed);
    }

    public void Move(Vector3 target)
    {
        TargetPosition = target;
    }
}
