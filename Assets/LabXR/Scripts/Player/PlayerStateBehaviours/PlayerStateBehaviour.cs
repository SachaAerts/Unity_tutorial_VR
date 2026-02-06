using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerStateBehaviour : MonoBehaviour
{
    public abstract void Init();

    public abstract void Tick();

    public abstract void Enter();

    public abstract void Exit();

    #region  Receive Inputs Callbacks

    public abstract void OnReceiveLeftJoystickAxis(InputAction.CallbackContext ctx);

    #endregion
}