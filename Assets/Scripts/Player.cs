using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;


//Using input actions by attaching to Player input invoking uity events
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float speed, holdTime = 1f;
    [SerializeField] GridNode gridMap;
    [SerializeField] LayerMask collisionMask;

    Animator _animator;
    Rigidbody2D _rb;
    Movement _movement;

    Vector2 _mousePosition;

    Vector2 _movementDirection;
    bool _hasMovementInput;
    float _movementInputPressedTime;
    IPathfinding _pathFinding;
    List<Node> _route;
    Coroutine _followPathRoutine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _movement = new Movement(speed);
        _pathFinding = new Astar();

        if (_rb == null)
            Debug.LogError("no rigidbody component attached");
        if (_animator == null)
            Debug.LogError("no animator component found in children");
        //if (gridMap == null)
        //    Debug.LogError("no grid map attached.");

    }
    #region Unity Methods
    private void Start()
    {
        _movement.DesiredPosition = _rb.position;
    }
    private void OnDrawGizmos()
    {
        if (_rb != null && _movementDirection != null && gridMap != null)
            Gizmos.DrawCube(_rb.position + _movementDirection, gridMap.TileSize / 2);

        if(_route != null)
        foreach (var path in _route)
        {
            Gizmos.DrawSphere(gridMap.GridToWorld(new Vector2Int(path.Coordinates.x, path.Coordinates.y)), gridMap.TileSize.x/2);
        }
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        ProcessMovementUpdate(_rb, _movementDirection, Time.fixedDeltaTime);
        _movement.MoveTowards2D(_rb, _movement.DesiredPosition, Time.fixedDeltaTime);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit enter");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Hit");
    }

    #endregion


    public void OnMove(InputAction.CallbackContext ctx)
    {
        ProcessMoveInput(ctx);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //var mPos = Camera.main.ScreenToWorldPoint(_mousePosition);
            //var hit = Physics2D.Raycast(mPos, Vector2.zero);

            //if (hit)
            //{
            //    Debug.Log($"layer hit {hit.transform.gameObject.layer}");
            //}
            //else
            //{
            //    Debug.Log("nothing hit");
            //}

            Vector2Int origin = gridMap.WorldToGrid(_rb.position);
            Debug.Log($"origin node connectedness {gridMap.Map[origin.x, origin.y].ConnectedValue}");
            var mPos = Camera.main.ScreenToWorldPoint(_mousePosition);
            Vector2Int destination = gridMap.WorldToGrid(mPos);
            Debug.Log($"destination node connectedness {gridMap.Map[destination.x, destination.y].ConnectedValue}");


            //gridMap.CheckForObstacles();
            //if (_followPathRoutine != null)
            //    StopCoroutine(_followPathRoutine);

            //_route = _pathFinding.FindPath(gridMap.Map[origin.x, origin.y], gridMap.Map[destination.x, destination.y]);

            //_followPathRoutine = StartCoroutine(FollowPath(_rb, _route));
        }
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("Paused");
    }

    public void OnMouseMove(InputAction.CallbackContext ctx)
    {
        //TODO: find a way to change this.
        Vector2 position = ctx.ReadValue<Vector2>();
        _mousePosition = position;
    }

    #region MovementHandler
    void ProcessMoveInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            _hasMovementInput = true;

        }

        if (ctx.performed)
        {
            _movementDirection = ctx.ReadValue<Vector2>().normalized;
            _movementDirection = _movement.RestrainDiagonalDirection(_movementDirection);
            Debug.Log($"performed {_movementDirection}");

        }

        if (ctx.canceled)
        {
            _hasMovementInput = false;
            Debug.Log($"cancelled {_hasMovementInput}");
        }
    }

    void ProcessMovementUpdate(Rigidbody2D rb, Vector2 direction, float fixedDeltaTime)
    {
        if (_hasMovementInput)
            _movementInputPressedTime += fixedDeltaTime;

        if (_movementInputPressedTime >= holdTime)
        {
            _movementInputPressedTime = 0;
            SetDesiredPosition(rb, direction);
        }
    }

    void SetDesiredPosition(Rigidbody2D rb, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, gridMap.TileSize, 0, _movementDirection, 1f, collisionMask);
        if (hit)
            return;
        _movement.DesiredPosition = gridMap.ToNearestTilePosition(rb.position + direction); // convert to grid location - just to make sure that it is aligned to a gird.
    }

    IEnumerator FollowPath(Rigidbody2D rb, List<Node> route)
    {
        if (route == null)
            yield break;

        if (route.Count <= 0)
            yield break;


        foreach (var path in route)
        {
            var node = path;

            Vector2 destination = gridMap.GridToWorld(new Vector2Int(node.Coordinates.x, node.Coordinates.y));
            SetDesiredPosition(rb, destination - rb.position);

            yield return new WaitUntil(() => _rb.position == destination);
            yield return new WaitForSeconds(holdTime);
        }

    }
    #endregion
}
