using UnityEngine;

public interface IMoverComponent
{
    float MaxSpeed { get; }
    float MaxSteering { get; }
    Rigidbody RigidBody { get; }
    Vector3 CurrentVelocity { get; }
    Vector3 CurrentPosition { get; }
    Vector3 LastDirectionFacing { get; }
}

public class MoverComponent : MonoBehaviour, IMoverComponent
{
    protected Rigidbody _rb;
    protected Movement _movement;
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected float _maxSteering;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public float MaxSpeed { get { return _maxSpeed; } }
    public float MaxSteering { get { return _maxSteering; } }
    public Rigidbody RigidBody { get { return _rb; } }
    public Vector3 CurrentVelocity { get { return _rb.velocity; } }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }
}

public interface IDirectionMoverComponent : IMoverComponent
{
    Vector3 Direction { get;}
    void MoveDirection(Vector3 direction);
}

public interface ITargetMoverComponent : IMoverComponent
{
    Vector3 TargetPosition { get;}
    void SetTargetPosition(Vector3 target);
}