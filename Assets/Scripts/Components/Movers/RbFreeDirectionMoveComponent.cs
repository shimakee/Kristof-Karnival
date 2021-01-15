using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbFreeDirectionMoveComponent : MonoBehaviour, IDirectionMoverComponent
{
    [SerializeField] float speed = 1;
    [SerializeField] bool ZAsY;

    //public Vector3 TargetPosition { get { return _movement.DesiredPosition; } set { _movement.DesiredPosition = value; } }
    public Vector3 Direction
    { 
        get { return _movement.Direction; } 
        set 
        { 
            _movement.Direction = value; 
            if (value.magnitude != 0)
            {
                if(ZAsY)
                    _movement.LastDirectionFacing = _movement.SwitchZandY(value.normalized); 
                else
                    _movement.LastDirectionFacing = value.normalized;
            }
        } 
    }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }


    Movement _movement;
    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = new Movement(speed);
        _movement.DesiredPosition = _rb.position;
    }

    private void FixedUpdate()
    {
        if (ZAsY)
            _movement.AssignVelocity(_rb, _movement.SwitchZandY(Direction));
        else
            _movement.AssignVelocity(_rb, Direction);
    }
}
