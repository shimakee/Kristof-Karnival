﻿using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;


//Using input actions by attaching to Player input invoking uity events
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float speed = 1;

    Animator _animator;
    Rigidbody2D _rb;
    Movement _movement;

    Vector2 _direction = Vector2.zero;
    Vector2 _desiredPosition;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _movement = new Movement(speed);

        if (_rb == null)
            Debug.LogError("no rigidbody component attached");
        if (_animator == null)
            Debug.LogError("no animator component found in children");
    }

    private void Start()
    {
        _desiredPosition = _rb.position;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        //_movement.AddForce2D(_rb, _direction, Time.deltaTime);
        //_movement.AssignVelocity2D(_rb, _direction);
        //_movement.MovePosition2D(_rb, _direction);
        _movement.MoveTowards2D(_rb, _desiredPosition, Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _direction = ctx.ReadValue<Vector2>();
        Debug.Log($"direction: {_direction}");

        //TODO: move to separate animation handler
        _animator.SetFloat("Direction", _direction.x);
        _animator.SetFloat("Magnitude", _direction.magnitude);

        if (ctx.started)
        {
            _desiredPosition = _rb.position + _direction;
            Debug.Log($"Desired position: {_desiredPosition}");
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Looking");
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("Paused");
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
