using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class RbTargetNavmeshMoveComponent : MonoBehaviour, ITargetMoverComponent
{
    [SerializeField] float speed = 1;
    [SerializeField] bool ignoreY;
    [SerializeField] bool useRigidBodyAsKinematic;

    public Vector3 TargetPosition
    {
        get { return _movement.DesiredPosition; }
        set
        {
            _movement.DesiredPosition = value;
            if (useRigidBodyAsKinematic)
                _agent.SetDestination(value);
            else
                StartCoroutine(ProcessPath(value));
        }
    }
    public Vector3 CurrentPosition { get { return _rb.position; } }
    public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }


    Movement _movement;
    Rigidbody _rb;
    Vector3 _targetAdjusted;

    NavMeshAgent _agent;
    Vector3[] _path;
    private void Awake()
    {
        _movement = new Movement(speed);
        _rb = GetComponent<Rigidbody>();
        _movement.DesiredPosition = _rb.position;

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
    }
    private void Start()
    {
        if (useRigidBodyAsKinematic)
        {
            _rb.isKinematic = true;
        }
        else
        {
            _agent.updateRotation = false;
            _agent.updatePosition = false;
            _agent.autoTraverseOffMeshLink = false;
            _agent.isStopped = true;
        }

    }

    private void FixedUpdate()
    {
        if (useRigidBodyAsKinematic)
            return;

        _targetAdjusted = _movement.DesiredPosition;

        if (ignoreY)
        {
            _targetAdjusted.y = _rb.position.y; // let gravity handle falling
            _movement.DesiredPosition = _targetAdjusted;
        }

        _movement.LastDirectionFacing = _targetAdjusted - _rb.position;
        if (_rb.position != _movement.DesiredPosition)
            _movement.RotateForwardDirectionInYAxis(_rb, _movement.LastDirectionFacing);
        _movement.MoveTowards(_rb, _targetAdjusted, Time.fixedDeltaTime);

        Debug.DrawLine(_rb.transform.position, _movement.LastDirectionFacing + _rb.transform.position, Color.blue);
    }

    IEnumerator ProcessPath(Vector3 destination)
    {
        _agent.path.ClearCorners(); 

        _agent.SetDestination(destination);

        yield return new WaitUntil(() => _agent.hasPath);

        int pathLength = _agent.path.corners.Length;
        _path = _agent.path.corners;

        for (int i = 0; i < pathLength; i++)
        {
            Vector3 path = _path[i];

            path.y = _rb.position.y;

            _movement.DesiredPosition = path;
            yield return new WaitUntil(() => _rb.position == _movement.DesiredPosition);
        }
        _agent.Warp(_rb.position);
        _agent.path.ClearCorners();

    }

    public void Move(Vector3 target)
    {
        TargetPosition = target;
    }
}
