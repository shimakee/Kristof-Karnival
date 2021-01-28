using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RbForceMoveComponent : MonoBehaviour, IForceMoverComponent
{
    [Range(0, 100)][SerializeField] float speed;
    [Range(0, 100)][SerializeField] float maxForceApplied;

    public Vector3 Direction { get { return _mover.Direction; } set { _mover.Direction = value; } }

    public Vector3 CurrentPosition { get { return _rb.position; } }

    public Vector3 LastDirectionFacing { get { return _mover.LastDirectionFacing; } private set { _mover.LastDirectionFacing = value; } }

    public Rigidbody Rigidbody { get { return _rb; } }


    Rigidbody _rb;
    Movement _mover;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mover = new Movement(speed);
    }


    public void AddForce(Vector3 force)
    {
        force = Vector3.ClampMagnitude(force, maxForceApplied);
        _mover.AddForce(_rb, force);
    }

    public void AssignVelocity(Vector3 velocity)
    {
        _mover.AssignVelocity(_rb, velocity);
    }
}
