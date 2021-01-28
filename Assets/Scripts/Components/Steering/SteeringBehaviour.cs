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

    [SerializeField] GameObject[] path;
    [SerializeField] float pathRadius;

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
        _direction += FollowPath(path[0].transform.position, path[1].transform.position);
        _direction = Arriving(_direction, target.transform.position, arrivingDistance);

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

    private Vector3 Arriving(Vector3 velocity, Vector3 target, float arrivingDistanceToTarget)
    {
        var distance = Vector3.Distance(target, _mover.CurrentPosition);
        float multiplier = (distance < arrivingDistanceToTarget) ? distance / arrivingDistanceToTarget : 1;

        return Vector3.ClampMagnitude(velocity, maxTravelSpeed * multiplier);

    }

    private Vector3 FollowPath(Vector3 firstPos, Vector3 secondPos)
    {
        Vector3 path = secondPos - firstPos;
        Vector3 playerPositionRelativeToPath = _mover.CurrentPosition - firstPos;
        Vector3 playerFuturePosition = playerPositionRelativeToPath + _direction;
        //Vector3 currentPositionAlongPath = path.normalized * Vector3.Dot(player, path.normalized);
        Vector3 futurePositionAlongPath = path.normalized * Vector3.Dot(playerFuturePosition, path.normalized);

        float distance = Vector3.Distance(futurePositionAlongPath, playerFuturePosition);

        Debug.DrawLine(firstPos, futurePositionAlongPath + firstPos);
        Debug.DrawLine(futurePositionAlongPath, playerFuturePosition);

        if (distance > pathRadius)
            return Seek(futurePositionAlongPath + firstPos, _mover.CurrentPosition);
        else
            return Vector3.zero;
    }
    #endregion
}
