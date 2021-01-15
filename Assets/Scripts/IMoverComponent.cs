using UnityEngine;

public interface IMoverComponent
{
    Vector3 CurrentPosition { get; }
    Vector3 LastDirectionFacing { get; }
}

public interface IDirectionMoverComponent : IMoverComponent
{
    Vector3 Direction { get; set; }
}

public interface ITargetMoverComponent : IMoverComponent
{
    Vector3 TargetPosition { get; set; }
}