using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class FocusStateBehaviour : PlayerStateBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Distance Settings")]
    [SerializeField] private float distanceSpeed = 2f;
    [SerializeField] private float minDistance = 0.5f;   
    [SerializeField] private float maxDistance = 10f;

    [Header("VR Locomotion")]
    [SerializeField] private SnapTurnProvider snapTurnProvider;

    [Header("Controllers")]
    [SerializeField] private Transform leftControllerPlayer;
    [SerializeField] private Transform leftHandPlayer;
    [SerializeField] private Transform rightHandPlayer;

    private float currentDistance;     
    private Vector2 leftJoystickAxisValue;
    private Vector2 rightJoystickAxisValue;
    private Transform currentFocusTransform;
    private Toy selectedToy;
    private bool isRightHandPinching = false;
    private float initialTwoHandDistance;
    private Quaternion initialRightHandRotation;
    private Quaternion initialToyRotation;

    public void SetFocusTransform(Transform reference)
    {
        currentFocusTransform = reference;
        currentDistance = Vector3.Distance(leftControllerPlayer.position, currentFocusTransform.position);
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
        
        isRightHandPinching = false;
    }

    public override void Init(PlayerController reference)
    {
        base.Init(reference);
    }

    public override void Tick(Enums.ControllerState currentControllerState)
    {
        switch (currentControllerState)
        {
            case Enums.ControllerState.Controller:
                TickWithController();
                break;
            case Enums.ControllerState.Hand:
                TickWithHand();
                break;
        }
    }

    private void TickWithController()
    {
        if (currentFocusTransform == null || selectedToy == null) return;

        HandleDistance();
        HandleRotation();
    }

    private void TickWithHand()
    {
        if (currentFocusTransform == null || selectedToy == null) return;

        if (isRightHandPinching)
        {
            HandleTwoHandManipulation();
        }
        else
        {
            HandleDistanceWithHand();
        }
    }

    private void HandleDistance()
    {
        if (Mathf.Abs(rightJoystickAxisValue.y) > 0.01f)
        {
            float distanceChange = rightJoystickAxisValue.y * distanceSpeed * Time.deltaTime;
            currentDistance += distanceChange;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }
        
        currentFocusTransform.position = leftControllerPlayer.position + leftControllerPlayer.forward * currentDistance;
    }

    private void HandleDistanceWithHand()
    {
        currentFocusTransform.position = leftHandPlayer.position + leftHandPlayer.forward * currentDistance;
    }

    private void HandleRotation()
    {
        float horizontalRotation = leftJoystickAxisValue.x * rotationSpeed * Time.deltaTime;
        float verticalRotation = leftJoystickAxisValue.y * rotationSpeed * Time.deltaTime;
        
        if (Mathf.Abs(leftJoystickAxisValue.x) > 0.1f)
        {
            currentFocusTransform.Rotate(player.transform.up, -horizontalRotation, Space.World); 
        }
        
        if (Mathf.Abs(leftJoystickAxisValue.y) > 0.1f)
        {
            currentFocusTransform.Rotate(player.transform.right, verticalRotation, Space.World);  
        }
    }

    #region Two-Hand Manipulation

    private void HandleTwoHandManipulation()
    {
        currentFocusTransform.position = leftHandPlayer.position + leftHandPlayer.forward * currentDistance;

        HandleTwoHandRotation();
        HandleTwoHandZoom();
    }

    private void HandleTwoHandRotation()
    {
        Quaternion currentRightRotation = rightHandPlayer.rotation;
        Quaternion deltaRotation = currentRightRotation * Quaternion.Inverse(initialRightHandRotation);
        
        currentFocusTransform.rotation = deltaRotation * initialToyRotation;
        
        if (Time.frameCount % 30 == 0) 
        {
            Debug.Log($"Rotation main droite : {currentRightRotation.eulerAngles} | Rotation objet : {currentFocusTransform.rotation.eulerAngles}");
        }
    }

    private void HandleTwoHandZoom()
    {
        float currentTwoHandDistance = Vector3.Distance(leftHandPlayer.position, rightHandPlayer.position);
        float zoomRatio = currentTwoHandDistance / initialTwoHandDistance;

        float newDistance = currentDistance * zoomRatio;
        currentDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        
        initialTwoHandDistance = currentTwoHandDistance;
    }

    #endregion

    #region Receive Inputs Callbacks   

    public override void OnReceiveLeftJoystickAxis(InputAction.CallbackContext ctx)
    {
        leftJoystickAxisValue = ctx.ReadValue<Vector2>();
    }

    public override void OnReceiveRightJoystickAxis(InputAction.CallbackContext ctx)
    {
        rightJoystickAxisValue = ctx.ReadValue<Vector2>();
    }

    public void OnReceiveSelectEnteredToy(Toy args)
    {
        selectedToy = args;
        leftJoystickAxisValue = new (0, 0);
        
        if (selectedToy.IsTargeted)
        {
            selectedToy.IsSelected = true;
            selectedToy.rigidbodyToy.isKinematic = true;
        }
    }

    public void OnReceiveSelectExitedToy()
    {
        if (selectedToy != null)
        {
            selectedToy.IsSelected = false;
            selectedToy.rigidbodyToy.isKinematic = false;
        }
        
        isRightHandPinching = false;
    }

    #endregion

    #region Right Pinch

    public void OnRightPinchStarted()
    {
        isRightHandPinching = true;
        
        initialTwoHandDistance = Vector3.Distance(leftHandPlayer.position, rightHandPlayer.position);
        initialRightHandRotation = rightHandPlayer.rotation;
        initialToyRotation = currentFocusTransform.rotation;
    }

    public void OnRightPinchEnded()
    {
        isRightHandPinching = false;
    }

    public void OnRightPinchHeld()
    {
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log("Right pinch maintenu - manipulation en cours");
        }
    }

    #endregion
}