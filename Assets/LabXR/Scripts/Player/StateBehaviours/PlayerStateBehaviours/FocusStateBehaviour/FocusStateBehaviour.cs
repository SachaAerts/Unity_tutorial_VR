using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class FocusStateBehaviour : PlayerStateBehaviour
{
    [Header("VR Locomotion")]
    [SerializeField] private SnapTurnProvider snapTurnProvider;

    [Header("Controllers")]
    [SerializeField] private Transform leftControllerPlayer;
    [SerializeField] private Transform leftHandPlayer;
    [SerializeField] private Transform rightHandPlayer;

    public FocusStateActions FocusStateActions;

    public void SetFocusTransform(Transform reference)
    {
        FocusStateActions.FocusState.CurrentFocusTransform = reference;
        FocusStateActions.DistanceState.CurrentDistance = Vector3.Distance(leftControllerPlayer.position, 
                                                            FocusStateActions.FocusState.CurrentFocusTransform.position);
    }

    public override void Enter()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.enabled = false;
    }

    public override void Exit()
    {
        if (snapTurnProvider != null)
            snapTurnProvider.enabled = true;
        
        FocusStateActions.IsRightHandPinching = false;
    }

    public override void Init(PlayerController reference)
    {
        base.Init(reference);
        FocusStateActions = new ();
        FocusStateActions.InitStates();
    }

    public override void Tick(Enums.ControllerState currentControllerState)
    {
        switch (currentControllerState)
        {
            case Enums.ControllerState.Controller:
                FocusStateActions.TickWithController(leftControllerPlayer, player);
                break;
            case Enums.ControllerState.Hand:
                FocusStateActions.TickWithHand(leftHandPlayer, rightHandPlayer);
                break;
        }
    }

    #region Receive Inputs Callbacks   

    public override void OnReceiveLeftJoystickAxis(InputAction.CallbackContext ctx)
    {
        FocusStateActions.JoysticksState.LeftJoystickAxisValue = ctx.ReadValue<Vector2>();
    }

    public override void OnReceiveRightJoystickAxis(InputAction.CallbackContext ctx)
    {
        FocusStateActions.JoysticksState.RightJoystickAxisValue = ctx.ReadValue<Vector2>();
    }

    public void OnReceiveSelectEnteredToy(Toy args)
    {
        FocusStateActions.FocusState.SelectedToy = args;
        FocusStateActions.JoysticksState.LeftJoystickAxisValue = new (0, 0);
        
        if (FocusStateActions.FocusState.SelectedToy.IsTargeted)
        {
            FocusStateActions.FocusState.SelectedToy.IsSelected = true;
            FocusStateActions.FocusState.SelectedToy.rigidbodyToy.isKinematic = true;
        }
    }

    public void OnReceiveSelectExitedToy()
    {
        if (FocusStateActions.FocusState.SelectedToy != null)
        {
            FocusStateActions.FocusState.SelectedToy.IsSelected = false;
            FocusStateActions.FocusState.SelectedToy.rigidbodyToy.isKinematic = false;
        }
        
        FocusStateActions.IsRightHandPinching = false;
    }

    #endregion

    #region Right Pinch

    public void OnRightPinchStarted()
        => FocusStateActions.StartRightPinch(leftHandPlayer, rightHandPlayer);

    public void OnRightPinchEnded()
        => FocusStateActions.EndRightPinch();

    #endregion
}