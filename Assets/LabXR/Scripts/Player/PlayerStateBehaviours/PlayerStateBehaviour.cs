using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerStateBehaviour : MonoBehaviour
{
    protected PlayerController player;

    public virtual void Init(PlayerController reference)
    {
        player = reference;
    }

    public abstract void Tick(Enums.ControllerState currentControllerState);

    public abstract void Enter();

    public abstract void Exit();

    #region  Receive Inputs Callbacks

    public abstract void OnReceiveLeftJoystickAxis(InputAction.CallbackContext ctx);

    public abstract void OnReceiveRightJoystickAxis(InputAction.CallbackContext ctx);

    #endregion
}