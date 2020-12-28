using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.Tilemaps;


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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _movement = new Movement(speed);

        if (_rb == null)
            Debug.LogError("no rigidbody component attached");
        if (_animator == null)
            Debug.LogError("no animator component found in children");
    }

    private void Start()
    {
        _movement.DesiredPosition = _rb.position;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        ProcessMovementUpdate(_rb, _movementDirection, Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        ProcessMoveInput(ctx);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            //var gridPos = gridMap.ScreenToGrid(_mousePosition);
            var gridPos = gridMap.WorldToGrid(_rb.position);
            Debug.Log($"grid pos: {gridPos}");
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
            Debug.Log($"started {_hasMovementInput}");
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
            Debug.Log($"Execute {_movementInputPressedTime}");
            _movement.DesiredPosition = rb.position + direction; // convert to grid location
            Debug.Log($"Change desiredPosition {_movement.DesiredPosition}, direction {direction}");
        }

        //Debug.Log($"moving {DesiredPosition}");
        _movement.MoveTowards2D(_rb, _movement.DesiredPosition, fixedDeltaTime);
    }
    #endregion
}
