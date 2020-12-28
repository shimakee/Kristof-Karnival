using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


//Using input actions by attaching to Player input invoking uity events
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float speed, holdTime = 1f;
    [SerializeField] NodeGridmapComponent gridMap;

    Animator _animator;
    Rigidbody2D _rb;
    Movement _movement;

    Vector2 _mousePosition;

    Vector2 _movementDirection;
    bool _hasMovementInput;
    float _movementInputPressedTime;
    AstarPathfinding _pathFinding;
    Queue<Node> route;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _movement = new Movement(speed);
        _pathFinding = new AstarPathfinding();

        if (_rb == null)
            Debug.LogError("no rigidbody component attached");
        if (_animator == null)
            Debug.LogError("no animator component found in children");
        if (gridMap == null)
            Debug.LogError("no grid map attached.");

    }
    #region Unity Methods
    private void Start()
    {
        _movement.DesiredPosition = _rb.position;
    }
    private void OnDrawGizmos()
    {
        if (_rb != null && _movementDirection != null && gridMap != null)
            Gizmos.DrawCube(_rb.position + _movementDirection, gridMap.TilSize / 2);

        if(route != null)
        foreach (var path in route)
        {
            Debug.Log($"path position:{path.x} {path.y}");

            Gizmos.DrawSphere(gridMap.GridToWord(new Vector2Int(path.x, path.y)), gridMap.TilSize.x/2);
        }
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        ProcessMovementUpdate(_rb, _movementDirection, Time.fixedDeltaTime);
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
            Vector2Int origin = gridMap.WorldToGrid(_rb.position);
            Debug.Log($"origin {origin}");
            Vector2Int destination = gridMap.ScreenToGrid(_mousePosition);
            Debug.Log($"destination {destination}");

            //gridMap.CheckForObstacles();
            route = _pathFinding.FindPath(gridMap.Map[origin.x, origin.y], gridMap.Map[destination.x, destination.y]);
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit enter");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Hit");
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
            RaycastHit2D hit = Physics2D.BoxCast(rb.position, gridMap.TilSize, 0, _movementDirection, 1f, LayerMask.GetMask("Tile Obstacles"));
            if (!hit)
                _movement.DesiredPosition = gridMap.ToNearestTilePosition(rb.position + direction); // convert to grid location - just to make sure that it is aligned to a gird.
        }
            _movement.MoveTowards2D(_rb, _movement.DesiredPosition, fixedDeltaTime);
    }
    #endregion
}
