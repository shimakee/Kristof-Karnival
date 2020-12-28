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

    Vector2 _direction;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _movement = new Movement(speed, holdTime);

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
        //_movement.MoveToDesiredPosition2D(_rb, Time.deltaTime);
        _movement.ProcessMovement2D(_rb, Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {

        _movement.ProcessMovementInput(ctx);

        //_direction = ctx.ReadValue<Vector2>();
        //Debug.Log($"@@@@@direction {_direction}");

        //if (ctx.started)
        //{
        //    Debug.Log("===>started");
        //    if (ctx.interaction is HoldInteraction)
        //        Debug.Log("Hold interaction");

        //    if (ctx.interaction is PressInteraction)
        //        Debug.Log("Press interaction");



        //}
        //if (ctx.performed)
        //{
        //    Debug.Log("===>performed");

        //    if (ctx.interaction is HoldInteraction)
        //        Debug.Log($"Hold interaction {_direction}");

        //    if (ctx.interaction is PressInteraction)
        //        Debug.Log($"Press interaction {_direction}");

        //}
        //if (ctx.canceled)
        //{
        //    Debug.Log("===>cancelled");

        //    if (ctx.interaction is HoldInteraction)
        //        Debug.Log($"Hold interaction {_direction}");

        //    if (ctx.interaction is PressInteraction)
        //        Debug.Log($"Press interaction {_direction}");

        //}

        //if (ctx.interaction is HoldInteraction)
        //{
        //    Debug.Log("*****Hold interaction");

        //    if (ctx.started)
        //        Debug.Log($"started {_direction}");
        //    if (ctx.performed)
        //        Debug.Log($"performed {_direction}");
        //    if (ctx.canceled)
        //        Debug.Log($"cancelled {_direction}");

        //}
        //if (ctx.interaction is PressInteraction)
        //{
        //    Debug.Log("*****Press interaction");

        //    if (ctx.started)
        //        Debug.Log($"started {_direction}");
        //    if (ctx.performed)
        //        Debug.Log($"performed {_direction}");
        //    if (ctx.canceled)
        //        Debug.Log($"cancelled {_direction}");
        //}


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
}
