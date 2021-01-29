using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PathfindToTargetMove2DComponent : MoverComponent, ITargetMoverComponent
{
    [SerializeField] float speed = 1;
    [SerializeField] float tileIdleTime = .5f;
    [SerializeField] GridMap2DInherited gridMap;
    [SerializeField] LayerMask pathMask;


    //public Vector3 CurrentPosition { get { return _rb.position; } }
    ////public Vector3 Direction { get { return _movement.Direction; } set { _movement.Direction = value; } }
    //public Vector3 LastDirectionFacing { get { return _movement.LastDirectionFacing; } }
    public Vector3 TargetPosition
    { 
        get { return _movement.DesiredPosition; } 
        set 
        { 
            _movement.DesiredPosition = value;
            CalculatePath();
        } 
    }

    //Movement _movement;
    new Rigidbody2D _rb;

    IPathfinding _pathfinding;
    List<IUnityPathNode> _route;
    Vector2 _path;
    Coroutine _routeJourney;

    private void Awake()
    {
        _movement = new Movement(speed);
        _rb = GetComponent<Rigidbody2D>();
        _movement.DesiredPosition = CurrentPosition;
        _path = CurrentPosition;
        _pathfinding = new Astar();
        //_route = new List<IUnityPathNode>();
    }

    private void FixedUpdate()
    {
        _movement.MoveTowards2D(_rb, _path, Time.fixedDeltaTime * speed);
    }

    void CalculatePath()
    {
        var origin = gridMap.GetGridObject(gridMap.WorldToGrid(CurrentPosition));
        var destination = gridMap.GetGridObject(gridMap.WorldToGrid(TargetPosition));

        _route = _pathfinding.FindPath(origin, destination, pathMask);

        if (_routeJourney != null)
            StopCoroutine(_routeJourney);
        _routeJourney = StartCoroutine(MoveToPath(_route));
    }

    IEnumerator MoveToPath(IList<IUnityPathNode> route)
    {
        if (route == null)
            yield break;
        if (route.Count == 0)
            yield break;
        while (route.Count > 0)
        {
            _path = gridMap.GetNearestTilePosition(route[0].WorldPosition);

            _movement.LastDirectionFacing = _path - _rb.position;

            yield return new WaitUntil(()=> _rb.position == _path);
            yield return new WaitForSeconds(tileIdleTime);
            route.RemoveAt(0);
        }
    }

    public void SetTargetPosition(Vector3 target)
    {
        TargetPosition = target;
    }
}
