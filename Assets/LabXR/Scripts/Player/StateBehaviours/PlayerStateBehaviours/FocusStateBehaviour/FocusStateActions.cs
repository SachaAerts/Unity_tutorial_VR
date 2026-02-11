using UnityEngine;
using UnityEngine.InputSystem;
public class FocusStateActions
{   
    public FocusState FocusState;
    public DistanceState DistanceState;
    public JoysticksState JoysticksState;
    public bool IsRightHandPinching = false;
    private RotationState rotationState;

    public void InitStates()
    {
        FocusState = new ();
        DistanceState = new ();
        JoysticksState = new ();
        rotationState = new ();
    }
    
    public void TickWithController(Transform leftControllerPlayer, PlayerController player)
    {
        if (FocusState.CurrentFocusTransform == null || FocusState.SelectedToy == null) return;

        HandleDistance(leftControllerPlayer);
        HandleRotation(player);
    }

    public void TickWithHand(Transform leftHandPlayer, Transform rightHandPlayer)
    {
        if (FocusState.CurrentFocusTransform == null || FocusState.SelectedToy == null) return;

        if (IsRightHandPinching)
        {
            HandleTwoHandManipulation(leftHandPlayer, rightHandPlayer);
        }
        else
        {
            HandleDistanceWithHand(leftHandPlayer);
        }
    }

    private void HandleDistance(Transform leftControllerPlayer)
    {
        if (Mathf.Abs(JoysticksState.RightJoystickAxisValue.y) > 0.01f)
        {
            float distanceChange = JoysticksState.RightJoystickAxisValue.y * DistanceState.DistanceSpeed * Time.deltaTime;
            DistanceState.CurrentDistance += distanceChange;
            DistanceState.CurrentDistance = Mathf.Clamp(DistanceState.CurrentDistance, DistanceState.MinDistance, DistanceState.MaxDistance);
        }
        
        FocusState.CurrentFocusTransform.position = leftControllerPlayer.position + leftControllerPlayer.forward * DistanceState.CurrentDistance;
    }

    private void HandleDistanceWithHand(Transform leftHandPlayer)
    {
        FocusState.CurrentFocusTransform.position = leftHandPlayer.position + leftHandPlayer.forward * DistanceState.CurrentDistance;
    }

    private void HandleRotation(PlayerController player)
    {
        float horizontalRotation = JoysticksState.LeftJoystickAxisValue.x * rotationState.RotationSpeed * Time.deltaTime;
        float verticalRotation = JoysticksState.LeftJoystickAxisValue.y * rotationState.RotationSpeed * Time.deltaTime;
        
        if (Mathf.Abs(JoysticksState.LeftJoystickAxisValue.x) > 0.1f)
        {
            FocusState.CurrentFocusTransform.Rotate(player.transform.up, -horizontalRotation, Space.World); 
        }
        
        if (Mathf.Abs(JoysticksState.LeftJoystickAxisValue.y) > 0.1f)
        {
            FocusState.CurrentFocusTransform.Rotate(player.transform.right, verticalRotation, Space.World);  
        }
    }

    #region Two-Hand Manipulation

    private void HandleTwoHandManipulation(Transform leftHandPlayer, Transform rightHandPlayer)
    {
        FocusState.CurrentFocusTransform.position = leftHandPlayer.position + leftHandPlayer.forward * DistanceState.CurrentDistance;

        HandleTwoHandRotation(rightHandPlayer);
        HandleTwoHandZoom(leftHandPlayer, rightHandPlayer);
    }

    private void HandleTwoHandRotation(Transform rightHandPlayer)
    {
        Quaternion currentRightRotation = rightHandPlayer.rotation;
        Quaternion deltaRotation = currentRightRotation * Quaternion.Inverse(rotationState.InitialRightHandRotation);
        
        FocusState.CurrentFocusTransform.rotation = deltaRotation * rotationState.InitialToyRotation;
    }

    private void HandleTwoHandZoom(Transform leftHandPlayer, Transform rightHandPlayer)
    {
        float currentTwoHandDistance = Vector3.Distance(leftHandPlayer.position, rightHandPlayer.position);
        float zoomRatio = currentTwoHandDistance / DistanceState.InitialTwoHandDistance;

        float newDistance = DistanceState.CurrentDistance * zoomRatio;
        DistanceState.CurrentDistance = Mathf.Clamp(newDistance, DistanceState.MinDistance, DistanceState.MaxDistance);
        
        DistanceState.InitialTwoHandDistance = currentTwoHandDistance;
    }

    #endregion

    #region Right pinch

    public void StartRightPinch(Transform leftHandPlayer, Transform rightHandPlayer)
    {
        IsRightHandPinching = true;
        
        DistanceState.InitialTwoHandDistance = Vector3.Distance(leftHandPlayer.position, rightHandPlayer.position);
        rotationState.InitialRightHandRotation = rightHandPlayer.rotation;
        rotationState.InitialToyRotation = FocusState.CurrentFocusTransform.rotation;
    }

    public void EndRightPinch()
        => IsRightHandPinching = false;

    #endregion
}