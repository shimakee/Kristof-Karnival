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

    private void Awake()
    {
    }
    #region Unity Methods

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit enter");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Hit");
    }

    #endregion


    public void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("Paused");
    }
}
