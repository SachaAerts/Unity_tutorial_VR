using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// PlayerController is the Singleton responsible for handling the player inputs and basics behaviour
/// </summary>
public class PlayerController : MonoSingleton<PlayerController>
{
    [Header("States")]
    public PlayerStateManager PlayerStateManager;
    public ControllerStateBehaviours ControllerStateBehaviour;
    
    [Header("Actions")]
    public RecenterComponent Recenter;
    public TeleportationComponent Teleportation;

    [Header("Hand Tracking")]
    [SerializeField] private Transform rightHandPlayer;

    public void Init()
    {
        PlayerStateManager.Init(this);
        Recenter.Init();        
    }

    public void Tick()
    {
        Recenter.UpdateRecenter();

        switch (PlayerStateManager.CurrentPlayerState)
        {
            case Enums.PlayerState.Observation:
                PlayerStateManager.ObservationStateBehaviour.Tick(ControllerStateBehaviour.CurrentControllerState);
                Teleportation.UpdateRightHandTeleportation();
                break;
            case Enums.PlayerState.Focus:
                PlayerStateManager.FocusStateBehaviour.Tick(ControllerStateBehaviour.CurrentControllerState);
                break;
        }
    }

    #region Right Pinch Detector

    private void OnRightPinchStarted()
    {
        if (PlayerStateManager.IsCurrentStateEqualsFocus())
        {
            PlayerStateManager.FocusStateBehaviour.OnRightPinchStarted();
        }
    }

    private void OnRightPinchEnded()
    { 
        if (PlayerStateManager.IsCurrentStateEqualsFocus())
        {
            PlayerStateManager.FocusStateBehaviour.OnRightPinchEnded();
        }
    }

    #endregion

    #region Input Actions Callbacks

    public void OnLeftJoystickAxis(InputAction.CallbackContext ctx)
    {
        switch (PlayerStateManager.CurrentPlayerState)
        {
            case Enums.PlayerState.Observation:
                PlayerStateManager.ObservationStateBehaviour.OnReceiveLeftJoystickAxis(ctx);
                break;
            case Enums.PlayerState.Focus:
                PlayerStateManager.FocusStateBehaviour.OnReceiveLeftJoystickAxis(ctx);
                break;
        }
    }

    public void OnRightJoystickAxis(InputAction.CallbackContext ctx)
    {
        switch (PlayerStateManager.CurrentPlayerState)
        {
            case Enums.PlayerState.Observation:
                PlayerStateManager.ObservationStateBehaviour.OnReceiveRightJoystickAxis(ctx);
                break;
            case Enums.PlayerState.Focus:
                PlayerStateManager.FocusStateBehaviour.OnReceiveRightJoystickAxis(ctx);
                break;
        }
    }

    public void OnTriggerButtonA(InputAction.CallbackContext ctx)
    {
        if (ctx.started) Recenter.RecenterPlayer();
    }

    public void OnTriggerButtonB(InputAction.CallbackContext ctx)
    {
        Debug.Log("Trigger Button B");
    }

    public void OnTriggerButtonX(InputAction.CallbackContext ctx)
    {
        Debug.Log("Trigger Button X");
    }

    public void OnTriggerButtonY(InputAction.CallbackContext ctx)
    {
        Debug.Log("Trigger Button Y");
    }

    public void OnRightPinch(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            OnRightPinchStarted();
        }
        else if (ctx.canceled)
        {
            OnRightPinchEnded();
        }
    }

    public void OnLeftPinch(InputAction.CallbackContext ctx)
    {
        Debug.Log("Left Pinch");
    }

    public void OnRightGrip(InputAction.CallbackContext ctx)
    {
        Debug.Log("Right Grip");
    }

    public void OnLeftGrip(InputAction.CallbackContext ctx)
    {
        Debug.Log("Left Grip");
    }

    public void OnRightTrigger(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Right Trigger {ctx.ReadValue<float>()}");
    }

    public void OnLeftTrigger(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Left Trigger {ctx.ReadValue<float>()}");
    }

    #endregion

    #region Input Interactor Callback

    public void OnSelectEnteredToy(SelectEnterEventArgs args)
    {
        GameObject selectedObject = args.interactableObject.transform.gameObject;
        if (!selectedObject.CompareTag("Toy") || selectedObject == null) return;
        Toy selectedToy = selectedObject.GetComponent<Toy>();

        PlayerStateManager.FocusStateBehaviour.SetFocusTransform(selectedToy.transform);
        PlayerStateManager.SetState(Enums.PlayerState.Focus);

        PlayerStateManager.FocusStateBehaviour.OnReceiveSelectEnteredToy(selectedToy);
    }

    public void OnSelectExitedToy(SelectExitEventArgs args)
    {
        PlayerStateManager.SetState(Enums.PlayerState.Observation);  
        PlayerStateManager.FocusStateBehaviour.OnReceiveSelectExitedToy();
    }
    
    #endregion
}