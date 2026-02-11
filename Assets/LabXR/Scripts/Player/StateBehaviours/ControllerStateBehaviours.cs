using UnityEngine;

public class ControllerStateBehaviours: MonoBehaviour
{
    public Enums.ControllerState CurrentControllerState;

    public void OnTrackedHandModeStarted()
        => CurrentControllerState = Enums.ControllerState.Hand;

    public void OnTrackedHandModeEnded()
        => CurrentControllerState = Enums.ControllerState.Controller;

    public void OnMotionControllerModeStarted()
        => CurrentControllerState = Enums.ControllerState.Controller;

    public void OnMotionControllerModeEnded()
        => CurrentControllerState = Enums.ControllerState.Hand;
}