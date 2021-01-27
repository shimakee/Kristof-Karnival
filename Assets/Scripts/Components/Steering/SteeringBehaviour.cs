using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent))]
public class SteeringBehaviour : MonoBehaviour
{

    [SerializeField] GameObject target;
    [Range(0, 50)][SerializeField] float maxTravelSpeed;
    [Range(0, 50)] [SerializeField] float maxSteeringForce;
    [SerializeField] float arrivingDistance;

    IDirectionMoverComponent _mover;
    Vector3 _direction;
    private void Awake()
    {
        _mover = GetComponent<IDirectionMoverComponent>();
        _direction = Vector3.zero;
    }

    private void Update()
    {
        _direction += Seek(target.transform.position, _mover.CurrentPosition) * Time.deltaTime;

        _direction = Arriving(target.transform.position, arrivingDistance);

        _mover.MoveDirection(_direction);
    }

    private void FixedUpdate()
    {
    }

    #region Seek
    private Vector3 Seek(Vector3 targetPosition, Vector3 currentPosition)
    {
        var desired = targetPosition - currentPosition;
        desired = desired.normalized * maxTravelSpeed;

        var steering = desired - _direction;
        steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        return steering;
    }

    private Vector3 Arriving(Vector3 target, float arrivingDistanceToTarget)
    {
        var distance = Vector3.Distance(target, _mover.CurrentPosition);
        float multiplier = (distance < arrivingDistanceToTarget) ? distance / arrivingDistanceToTarget : 1;

        return Vector3.ClampMagnitude(_direction, maxTravelSpeed * multiplier);

    }
    #endregion
}
