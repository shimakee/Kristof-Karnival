using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;


//Using input actions by attaching to Player input invoking uity events
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    Vector2 _direction;
    float speed = 1;

    private void Awake()
    {
    }

    private void Start()
    {
        playerInput.SwitchCurrentActionMap("Explore mode");
    }


    public void OnMove(InputAction.CallbackContext ctx)
    {
        _direction = ctx.ReadValue<Vector2>();
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
