using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class RbTargetMoveComponent : MonoBehaviour, ITargetMoverComponent
{
    [SerializeField] float speed = 1;
    [SerializeField] bool ignoreY;

    public Vector3 TargetPosition 
    { 
        get { return _movement.DesiredPosition; } 
        set { 
            _movement.DesiredPosition = value;
        } 
    }
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

    private void FixedUpdate()
    {
        _targetAdjusted = _movement.DesiredPosition;

        if (ignoreY)
            _targetAdjusted.y = _rb.position.y; // let gravity handle falling
        
        _movement.LastDirectionFacing = _targetAdjusted - _rb.position;
        if(_rb.position != _movement.DesiredPosition)
            _movement.RotateForwardDirectionInYAxis(_rb, _movement.LastDirectionFacing);
        _movement.MoveTowards(_rb, _targetAdjusted, Time.fixedDeltaTime);

        Debug.DrawLine(_rb.transform.position, _movement.LastDirectionFacing + _rb.transform.position, Color.blue);
    }
}
