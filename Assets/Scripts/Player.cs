using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;


//Using input actions by attaching to Player input invoking uity events
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    Animator _animator;
    Vector2 _direction;
    float speed = 1;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
            Debug.Log("no animator component found in children");
    }

    private void Start()
    {
        playerInput.SwitchCurrentActionMap("Explore mode");
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        _direction = ctx.ReadValue<Vector2>();
        _animator.SetFloat("Direction", _direction.x);
        _animator.SetFloat("Magnitude", _direction.magnitude);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Looking");
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("Paused");
    }

    private void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
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
