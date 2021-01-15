using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbTargetMoveComponent : MonoBehaviour, ITargetMoverComponent
{
    [SerializeField] float speed = 1;
    [SerializeField] bool ignoreY;

    public Vector3 TargetPosition { get { return _movement.DesiredPosition; } set { _movement.DesiredPosition = value; } }
    //public Vector3 Direction { get { return _movement.Direction; } set { _movement.Direction = value; if (value.magnitude != 0) _movement.LastDirectionFacing = value.normalized; } }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }


    Movement _movement;
    Rigidbody _rb;
    Vector3 _targetAdjusted;

    private void Awake()
    {
        _movement = new Movement(speed);
        _rb = GetComponent<Rigidbody>();
        _movement.DesiredPosition = _rb.position;
    }
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _targetAdjusted = _movement.DesiredPosition;

        if (ignoreY)
            _targetAdjusted.y = _rb.position.y; // let gravity handle falling
        
        _movement.LastDirectionFacing = _targetAdjusted - _rb.position;
        _movement.MoveTowards(_rb, _targetAdjusted, Time.fixedDeltaTime);
    }
}
