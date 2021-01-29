using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent), typeof(IFieldOfView))]
public class SteeringBehaviour : MonoBehaviour
{

    [Header("Seek behaviour:")]
    [SerializeField] GameObject target;
    [Range(0, 50)][SerializeField] float maxTravelSpeed;
    [Range(0, 50)] [SerializeField] float maxSteeringForce;

    [Header("Arriving behaviour:")]
    [SerializeField] float distanceFromTargetToReduceSpeed;
    [SerializeField] float distanceFromTargetToStop;

    [Header("Path finding behaviour:")]
    [SerializeField] GameObject[] path;
    [SerializeField] float pathRadius;
    [SerializeField] bool loopPath = false;
    Vector3[] pathLocations;

    IFieldOfView _fieldOfView;
    IDirectionMoverComponent _mover;
    Vector3 _direction;
    private void Awake()
    {
        _fieldOfView = GetComponent<IFieldOfView>();
        _mover = GetComponent<IDirectionMoverComponent>();
        _direction = Vector3.zero;
    }

    private void Update()
    {
        //_direction += Seek(target.transform.position) * Time.deltaTime;
        //_direction += Flee(target.transform.position) * Time.deltaTime;
        //_direction += FollowAlongPaths(path) * Time.deltaTime;

        _direction += Align(_fieldOfView.GameObjectsInView) * Time.deltaTime;

        //_direction = Arriving(_direction, target.transform.position, distanceFromTargetToReduceSpeed);

        _mover.MoveDirection(_direction);
    }

    #region Seek and Flee
    private Vector3 Seek(Vector3 targetPosition)
    {
        var desired = targetPosition - _mover.CurrentPosition;
            desired = desired.normalized * maxTravelSpeed;

        var steering = desired - _direction;
            steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        Debug.DrawLine(_mover.CurrentPosition, steering + _mover.CurrentPosition);

        return steering;
    }

    private Vector3 Flee(Vector3 targetPosition)
    {
        var desired = (targetPosition - _mover.CurrentPosition) * -1;
        desired = desired.normalized * maxTravelSpeed;

        var steering = desired - _direction;
        steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        Debug.DrawLine(_mover.CurrentPosition, steering + _mover.CurrentPosition);

        return steering;
    }
    #endregion

    #region Arriving behaviour
    private Vector3 Arriving(Vector3 velocity, Vector3 target, float arrivingDistanceToTarget)
    {
        var distance = Vector3.Distance(target, _mover.CurrentPosition) - distanceFromTargetToStop;
        if (distance < 0)
            distance = 0;
        float multiplier = (distance < arrivingDistanceToTarget) ? distance / arrivingDistanceToTarget : 1;

        return Vector3.ClampMagnitude(velocity, maxTravelSpeed * multiplier);

    }
    #endregion

    #region Path following
    //TODO: remove this to give actual node paths
    private Vector3 FollowAlongPaths(GameObject[] gameObjects)
    {
        pathLocations = new Vector3[gameObjects.Length];

        for (int i = 0; i < gameObjects.Length; i++)
        {
            pathLocations[i] = gameObjects[i].transform.position;
        }

        return FollowAlongPath(pathLocations);
    }

    private Vector3 FollowAlongPath(IList<Vector3> route)
    {
        int shortestDistance = 0;
        float distance = 0f;

        for (int i = 0; i < route.Count; i++)
        {
            if (!loopPath && i == route.Count - 2)
                break;

            var firstPos = pathLocations[i];
            var secondPos = (i >= pathLocations.Length - 1) ? pathLocations[0] : pathLocations[i + 1];

            Vector3 playerFuturePosition = PredictFuturePosition(_mover.CurrentPosition);
            var futurePositionAlongPath = PredictFuturePositionAlongPath(firstPos, secondPos);

            var distanceFromPath = Vector3.Distance(futurePositionAlongPath, playerFuturePosition);

            if (i == 0)
                distance = distanceFromPath;
            else if (distance > distanceFromPath)
            {
                shortestDistance = i;
                distance = distanceFromPath;
            }
        }

        Vector3 pathOne = pathLocations[shortestDistance];
        Vector3 pathTwo = (shortestDistance >= pathLocations.Length - 1) ? pathLocations[0] : pathLocations[shortestDistance + 1];

        return FollowPath(pathOne, pathTwo);
    }

    private Vector3 FollowPath(Vector3 firstPos, Vector3 secondPos)
    {
        Vector3 playerFuturePosition = PredictFuturePosition(_mover.CurrentPosition);
        Vector3 futurePositionAlongPath = PredictFuturePositionAlongPath(firstPos, secondPos);

        float distance = Vector3.Distance(futurePositionAlongPath, playerFuturePosition);

        Debug.DrawLine(firstPos, futurePositionAlongPath);
        Debug.DrawLine(firstPos, secondPos);
        Debug.DrawLine(futurePositionAlongPath, playerFuturePosition);

        if (distance > pathRadius)
            return Seek(futurePositionAlongPath);
        else
            return Vector3.zero;
    }

    private Vector3 PredictFuturePositionAlongPath(Vector3 firstPos, Vector3 secondPos)
    {
        Vector3 path = secondPos - firstPos;
        Vector3 playerPositionRelativeToPath = _mover.CurrentPosition - firstPos;
        Vector3 playerFuturePosition = PredictFuturePosition(playerPositionRelativeToPath);
        Vector3 futurePositionAlongPath = path.normalized * Vector3.Dot(playerFuturePosition, path.normalized);

        float dotProduct = Vector3.Dot(path, futurePositionAlongPath);

        futurePositionAlongPath = (dotProduct < 0) ? Vector3.ClampMagnitude(futurePositionAlongPath, 0) :
                                                    Vector3.ClampMagnitude(futurePositionAlongPath, path.magnitude);

        return futurePositionAlongPath + firstPos;
    }

    private Vector3 PredictFuturePosition(Vector3 currentPosition)
    {
        //return playerPositionRelativeToPath + _direction;
        return currentPosition + (_mover.LastDirectionFacing.normalized * maxTravelSpeed);
    }
    #endregion

    #region Separation



    #endregion

    #region Alignment & Cohesion
    private Vector3 Align(IList<GameObject> objectsInView)
    {
        Vector3 direction = Vector3.zero;

        for (int i = 0; i < objectsInView.Count; i++)
        {
            var component = objectsInView[i].GetComponent<IMoverComponent>();

            if(component != null)
                direction += component.CurrentVelocity;
        }

        if(objectsInView.Count > 0)
            direction = direction / objectsInView.Count;

        Debug.Log(direction);
        //return direction;
        return direction.normalized * maxTravelSpeed;
    }
        

    #endregion
}
