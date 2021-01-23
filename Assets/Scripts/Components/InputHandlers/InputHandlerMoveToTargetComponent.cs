using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandlerMousePositionComponent), typeof(ITargetMoverComponent))]
public class InputHandlerMoveToTargetComponent : InputHandlerComponent, IInputHandlerComponent
{
    [SerializeField] bool isOrthographic;
    [SerializeField] LayerMask maskRaycast;

    [SerializeField] bool enableDraw;

    InputHandlerMousePositionComponent _inputHandlerMousePosition;
    ITargetMoverComponent _mover;
    float _maxRayDistance = 1000;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (enableDraw && _mover != null)
            Gizmos.DrawWireSphere(_mover.TargetPosition, .1f);
    }

    protected override void InitializeOnAwake()
    {
        _inputHandlerMousePosition = GetComponent<InputHandlerMousePositionComponent>();
        _mover = GetComponent<ITargetMoverComponent>();
    }
    public override void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isOrthographic)
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(_inputHandlerMousePosition.MousePosition);
                _mover.TargetPosition = worldPos;
            }
            else
            {
                Vector3 nearPosition = new Vector3(_inputHandlerMousePosition.MousePosition.x,
                                                    _inputHandlerMousePosition.MousePosition.y,
                                                    Camera.main.nearClipPlane);
                Vector3 farPosition = new Vector3(_inputHandlerMousePosition.MousePosition.x,
                                                    _inputHandlerMousePosition.MousePosition.y,
                                                    Camera.main.farClipPlane);

                Vector3 nearRay = Camera.main.ScreenToWorldPoint(nearPosition);
                Vector3 farRay = Camera.main.ScreenToWorldPoint(farPosition);

                RaycastHit hitInfo;
                bool hasHit = Physics.Raycast(nearRay, farRay, out hitInfo, _maxRayDistance, maskRaycast, QueryTriggerInteraction.Ignore);

                if (hasHit)
                {
                    //Debug.Log(hitInfo.point);
                    _mover.TargetPosition = hitInfo.point;
                }
                else
                    Debug.Log("no hit");

                Debug.DrawLine(nearRay, farRay);
            }
        }
    }

}
