using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(IDirectionMoverComponent), typeof(IFieldOfView))]
public class SteeringBehaviourComponent : MonoBehaviour
{

    [Header("Seek behaviour:")]
    [SerializeField] bool enableSeekBehaviour;
    [SerializeField] float seekWeight;
    [SerializeField] GameObject target;
    [Range(0, 50)][SerializeField] float maxTravelSpeed;
    [Range(0, 50)] [SerializeField] float maxSteeringForce;
    [Space(10)]

    [Header("Flee behaviour:")]
    [SerializeField] bool enableFleeBehaviour;
    [SerializeField] float fleeWeight;
    [Space(10)]

    [Header("Evade behaviour:")]
    [SerializeField] bool enableEvadeBehaviour;
    [SerializeField] float evadeWeight;
    [Space(10)]

    [Header("Pursiut behaviour:")]
    [SerializeField] bool enablePursuitBehaviour;
    [SerializeField] float pursuitWeight;
    [SerializeField] float distanceAheadCutOff;
    [Space(10)]

    [Header("Wander behaviour:")]
    [SerializeField] bool enableWanderBehaviour;
    [SerializeField] float wanderWeight;
    [Range(0, 20)] [SerializeField] float wanderRadius;
    [Range(0, 20)] [SerializeField] float wanderDistance;
    [Range(0, 360)][SerializeField] float wanderAngle;
    [SerializeField] float wanderInterval;
    float _time;
    [Space(10)]

    [Header("Arriving behaviour:")]
    [SerializeField] bool enableArrivingBehaviour;
    [SerializeField] float distanceFromTargetToReduceSpeed;
    [SerializeField] float distanceFromTargetToStop;
    [Space(10)]

    [Header("Separation behaviour:")]
    [SerializeField] bool enableSeparationBehaviour;
    [SerializeField] float separationWeight;
    [SerializeField] float distanceToSeparate;
    [Space(10)]


    [Header("Path finding behaviour:")]
    [SerializeField] bool enablePathFollowingBehaviour;
    [SerializeField] float pathFollowingWeight;
    [SerializeField] GameObject[] path;
    [SerializeField] float pathRadius;
    [SerializeField] bool loopPath = false;
    [Space(10)]

    [Header("Alignment behaviour:")]
    [Space(10)]
    [SerializeField] bool enableAlignmentBehaviour;
    [SerializeField] float alignmentWeight;

    [Header("Cohesion behaviour:")]
    [SerializeField] bool enableCohesionBehaviour;
    [SerializeField] float cohesionWeight;
    [Space(10)]

    [Header("Collision avoidance behaviour:")]
    [SerializeField] bool enableCollisionAvoidance;
    [SerializeField] float collisionAvoidanceWeight;
    [SerializeField] LayerMask collisionDetectionMask;
    [SerializeField] float distanceAhead;
    [Space(10)]

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

        if (enableSeekBehaviour)
            _direction += Seek(target.transform.position) * Time.deltaTime * seekWeight;

        if (enableFleeBehaviour)
            _direction += Flee(target.transform.position) * Time.deltaTime * fleeWeight;

        if (enableEvadeBehaviour)
            _direction += Evade(target) * Time.deltaTime * evadeWeight;

        if (enableWanderBehaviour)
            _direction += WanderStart(wanderInterval, Time.deltaTime) * Time.deltaTime * wanderWeight;

        //if (enablePursuitBehaviour)
        //    _direction += Pursuit(target, distanceAheadCutOff) * Time.deltaTime * pursuitWeight;

        if (enablePathFollowingBehaviour)
            _direction += FollowAlongPaths(path) * Time.deltaTime * pathFollowingWeight;

        if (enableAlignmentBehaviour)
            _direction += Align(_fieldOfView.GameObjectsInView) * Time.deltaTime * alignmentWeight;

        if (enableCohesionBehaviour)
            _direction += Cohesion(_fieldOfView.GameObjectsInView) * Time.deltaTime * cohesionWeight;

        if (enableSeparationBehaviour)
            _direction += Separation(_fieldOfView.GameObjectsInSurroundings) * Time.deltaTime * separationWeight;

        if (enableCollisionAvoidance)
            _direction += CheckForCollision(distanceAhead, 90, 5) * Time.deltaTime * collisionAvoidanceWeight;

        if (enableArrivingBehaviour)
            _direction = Arriving(_direction, target.transform.position, distanceFromTargetToReduceSpeed);

        //_direction = Vector3.ClampMagnitude(_direction, maxTravelSpeed);
        //Debug.DrawLine(_mover.CurrentPosition, _mover.CurrentPosition + _direction);
        _mover.MoveDirection(_direction);
    }

    #region Steering
    private Vector3 Steer(Vector3 desiredDirection)
    {
        var steering = desiredDirection - _mover.CurrentVelocity;
        steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        return steering;
    }
    #endregion

    #region Seek and Flee
    private Vector3 Seek(Vector3 targetPosition)
    {
        var desired = targetPosition - _mover.CurrentPosition;
            desired = desired.normalized * maxTravelSpeed;

        //var steering = desired - _direction;
        //    steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        //Debug.DrawLine(_mover.CurrentPosition, steering + _mover.CurrentPosition);

        return Steer(desired);
    }

    private Vector3 Flee(Vector3 targetPosition)
    {
        var desired = (targetPosition - _mover.CurrentPosition) * -1;
        desired = desired.normalized * maxTravelSpeed;

        //var steering = desired - _direction;
        //steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        //Debug.DrawLine(_mover.CurrentPosition, steering + _mover.CurrentPosition);

        return Steer(desired);
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

        //Debug.DrawLine(firstPos, futurePositionAlongPath);
        //Debug.DrawLine(firstPos, secondPos);
        //Debug.DrawLine(futurePositionAlongPath, playerFuturePosition);

        if (distance > pathRadius)
            return Steer(futurePositionAlongPath - _mover.CurrentPosition);
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

    private Vector3 Separation(IList<GameObject> gameObjects)
    {
        Vector3 direction = Vector3.zero;
        int count = 0;
        for (int i = 0; i < gameObjects.Count; i++)
        {
            //var component = gameObjects[i].GetComponent<IMoverComponent>();
            Vector3 objectPosition = gameObjects[i].transform.position;
            Vector3 directionToObject = objectPosition - _mover.CurrentPosition;
            //float distance = directionToObject.magnitude;
            float distance = Vector3.Distance(objectPosition, _mover.CurrentPosition);

            if (distanceToSeparate > distance && distance > 0)
            {
                count++;
                direction += (Flee(objectPosition) / distance);
            }
        }

        if (count > 0)
            direction = direction / count;

        return direction;
        //return direction.normalized * maxTravelSpeed;

    }


    #endregion

    #region Alignment & Cohesion
    private Vector3 Align(IList<GameObject> objectsInView)
    {
        Vector3 direction = Vector3.zero;
        int count = 0;
        for (int i = 0; i < objectsInView.Count; i++)
        {
            var component = objectsInView[i].GetComponent<IMoverComponent>();

            if(component != null)
            {
                count++;
                direction += component.CurrentVelocity;
            }
        }

        if(count > 0)
            direction = direction / count;

        //Debug.Log(direction);
        return direction;
        //return direction.normalized * maxTravelSpeed;
    }

    private Vector3 Cohesion(IList<GameObject> objectsInView)
    {
        Vector3 direction = Vector3.zero;
        int count = 0;
        for (int i = 0; i < objectsInView.Count; i++)
        {
            var component = objectsInView[i].GetComponent<IMoverComponent>();

            if (component != null)
            {
                count++;
                //direction += component.CurrentPosition - _mover.CurrentPosition;
                direction += component.CurrentPosition;
            }
        }

        if (count > 0)
            direction = direction / count;

        //Debug.Log(direction);
        //return Seek(direction);
        return Steer(direction - _mover.CurrentPosition);
        //return direction.normalized * maxTravelSpeed;
    }


    #endregion

    #region Collision Avoidance

    private Vector3 CheckForCollision(float distance, float angle, int step)
    {
        Vector3 direction = _mover.CurrentVelocity;
        var dynamicLength = _mover.CurrentVelocity.magnitude / maxTravelSpeed;

        List<Vector3> directionsWithHit = new List<Vector3>();
        Vector3 summedDirection = Vector3.zero;
        for (int i = 0; i < step; i++)
        {
            float stepAngle = angle/step * i;

            var tempDirection = TransformVector(direction, stepAngle);
            var tempDirectionMirrored = TransformVector(direction, -stepAngle);
            Debug.DrawLine(_mover.CurrentPosition, _mover.CurrentPosition + tempDirection);
            Debug.DrawLine(_mover.CurrentPosition, _mover.CurrentPosition + tempDirectionMirrored);


            RaycastHit hitInfo;
            //bool isHit = Physics.Raycast(_mover.CurrentPosition, tempDirection, out hitInfo, distance, collisionDetectionMask);
            bool isHit = Physics.SphereCast(_mover.CurrentPosition, 1, tempDirection, out hitInfo, distance * dynamicLength, collisionDetectionMask);

            RaycastHit hitInfoMirrored;
            //bool isHitMirrored = Physics.Raycast(_mover.CurrentPosition, tempDirectionMirrored, out hitInfoMirrored, distance, collisionDetectionMask);
            bool isHitMirrored = Physics.SphereCast(_mover.CurrentPosition, 1, tempDirectionMirrored, out hitInfoMirrored, distance * dynamicLength, collisionDetectionMask);


            if (isHit)
                directionsWithHit.Add(hitInfo.point - _mover.CurrentPosition);
            if (isHitMirrored)
                directionsWithHit.Add(hitInfoMirrored.point - _mover.CurrentPosition);

            if(!isHit)
            {
                summedDirection += Steer(tempDirection.normalized * distance);
                break;
            }
            if (!isHitMirrored)
            {
                summedDirection += Steer(tempDirectionMirrored.normalized * distance);
                break;
            }
        }

        if(directionsWithHit.Count > 0)
        {
            var lowestMagnitudeVector = directionsWithHit.Aggregate((agg, next) => next.magnitude > agg.magnitude ? agg : next);
            summedDirection += Flee(lowestMagnitudeVector + _mover.CurrentPosition);

            //if (directionsWithHit.Count >= (step * 2) - 1)
            //{
            //    var highestMagnitudeVector = directionsWithHit.Aggregate((agg, next) => next.magnitude > agg.magnitude ? next : agg);
            //    summedDirection += Seek(highestMagnitudeVector + _mover.CurrentPosition);
            //}
        }

        //return summedDirection.normalized * maxTravelSpeed;
        return summedDirection;
    }

    private Vector3 TransformVector(Vector3 vector, float angle) //TODO: make based on angle
    {
        float cosin = Mathf.Cos(Mathf.Deg2Rad * angle);
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);

        float x = (vector.x * cosin) + (vector.z * -sin);
        float z = (vector.x * sin) + (vector.z * cosin);

        return new Vector3(x, vector.y, z);
    }
    #endregion

    #region Pursuit

    private Vector3 Pursuit(GameObject targetObject, float distanceAhead)
    {
        var component = targetObject.GetComponent<IMoverComponent>();

        if (component != null)
        {
            var target = component.CurrentPosition + component.RigidBody.velocity.normalized * distanceAhead;
            return Seek(target);
        }

        return Vector3.zero;
    }
    #endregion

    #region Evade
    private Vector3 Evade(GameObject targetObject)
    {
        var component = targetObject.GetComponent<IMoverComponent>();

        if (component != null)
        {
            var target = component.CurrentPosition + component.RigidBody.velocity;
            return Flee(target);
        }

        return Vector3.zero;
    }
    #endregion

    #region Wander

    private Vector3 WanderStart(float interval, float deltaTime)
    {
        _time += deltaTime;
        
        if (_time > interval)
        {
            _time = 0;
            return Wander(wanderDistance, wanderRadius, wanderAngle);
        }
        return Vector3.zero;
    }

    private Vector3 Wander(float distanceAhead, float radius, float angle)
    {
        var centerPoint = _mover.CurrentPosition + (_mover.LastDirectionFacing.normalized * distanceAhead);
        var destination = GetPointWithinACircle(centerPoint, radius, angle);

        Debug.DrawLine(_mover.CurrentPosition, destination);
        return Seek(destination);
    }

    Vector3 GetPointWithinACircle(Vector3 center, float radius, float maxAngle)
    {
        float randomAngle = Random.Range(-360, maxAngle);

        float x = center.x + Mathf.Cos(randomAngle * Mathf.Deg2Rad) * radius;
        float z = center.z + Mathf.Sin(randomAngle * Mathf.Deg2Rad) * radius;

        return new Vector3(x, center.y, z);
    }
    #endregion
}
