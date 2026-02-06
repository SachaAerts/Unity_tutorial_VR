using UnityEngine;
using UnityEngine.InputSystem;

public class FocusStateBehaviour : PlayerStateBehaviour
{
    private Vector2 joystickAxisValue;
    private Transform currentFocusTransform;

    public void SetFocusTransform(Transform reference)
    {
        currentFocusTransform = reference;
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Init()
    {
        
    }

    public override void Tick()
    {
        float angle = joystickAxisValue.x * 10 * Time.deltaTime;
        currentFocusTransform.Rotate(Vector3.up, angle, Space.World);    
    }

    #region  Receive Inputs Callbacks   

    public override void OnReceiveLeftJoystickAxis(InputAction.CallbackContext ctx)
    {
        joystickAxisValue = ctx.ReadValue<Vector2>();
    }

    #endregion
}
