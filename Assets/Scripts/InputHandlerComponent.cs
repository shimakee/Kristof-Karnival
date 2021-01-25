﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
[RequireComponent(typeof(PlayerInput))]
public abstract class InputHandlerComponent : MonoBehaviour, IInputHandlerComponent
{
    [SerializeField] string actionName = "default";
    [SerializeField] protected bool ZasY;
    public string ActionName { get { return actionName; } set { actionName = value; } }

    protected PlayerInput _playerInput;
    protected InputActionMap _inputActionMap;
    protected InputAction _inputAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inputActionMap = _playerInput.currentActionMap;
        _inputAction = _inputActionMap.FindAction(actionName);

        _inputAction.started += HandleAction;
        _inputAction.performed += HandleAction;
        _inputAction.canceled += HandleAction;

        InitializeOnAwake();
    }

    protected abstract void InitializeOnAwake();
    public abstract void HandleAction(InputAction.CallbackContext ctx);
}

public interface IInputHandlerComponent
{
    string ActionName { get; set; }
    void HandleAction(InputAction.CallbackContext ctx);
}