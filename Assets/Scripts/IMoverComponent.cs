using UnityEngine;

public interface IMoverComponent
{
    //Vector3 RigidBody { get; }
    Vector3 CurrentPosition { get; }
    Vector3 LastDirectionFacing { get; }
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