using UnityEngine.InputSystem;

public class ObservationStateBehaviour : PlayerStateBehaviour
{
    public override void Enter()
    {
        player.SetTeleportationEnabled(true);
    }

    public override void Exit()
    {
        player.SetTeleportationEnabled(false);
    }

    public override void Init(PlayerController reference)
    {
        base.Init(reference);
    }

    public override void Tick(Enums.ControllerState currentControllerState)
    {
        
    }

    #region  Receive Inputs Callbacks   

    public override void OnReceiveLeftJoystickAxis(InputAction.CallbackContext ctx)
    {
        
    }

    public override void OnReceiveRightJoystickAxis(InputAction.CallbackContext ctx)
    {
        
    }

    #endregion
}
