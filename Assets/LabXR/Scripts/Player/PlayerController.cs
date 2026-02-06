using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
/// PlayerController is the Singleton responsible for handling the player inputs and basics behaviour
/// </summary>
public class PlayerController : MonoSingleton<PlayerController>
{
    [Header("States")]
    public ObservationStateBehaviour ObservationStateBehaviour;
    public FocusStateBehaviour FocusStateBehaviour;

    private Enums.PlayerState currentState;

    [Header("Recenter")]
    public RecenterComponent Recenter;

    [Header("Right Hand Teleportation")]
    [SerializeField] TeleportationProvider teleportationProvider;
    [SerializeField] private XRRayInteractor rightHandTeleportationRayInteractor;
    [SerializeField] private bool isTeleportationEnabled = true;
    [SerializeField] private float rightHandTeleportationDotTolerance = .9f;
    [SerializeField] private float rightHandTeleportationWaitDuration = 2f;

    private float rightHandTeleportationWaitTimer;
    private bool isTeleporting;

    public void Init()
    {
        ObservationStateBehaviour.Init();
        FocusStateBehaviour.Init();
        Recenter.Init();

        currentState = Enums.PlayerState.Observation;
    }

    public void Tick()
    {
        Recenter.UpdateRecenter();

        switch (currentState)
        {
            case Enums.PlayerState.Observation:
                ObservationStateBehaviour.Tick();
                UpdateRightHandTeleportation();
                break;
            case Enums.PlayerState.Focus:
                FocusStateBehaviour.Tick();
                break;
        }
    }

    #region Behavioural State Machine

    public void SetState(Enums.PlayerState state)
    {
        ExitState(currentState);
        currentState = state;
        EnterState(currentState);
    }

    private void EnterState(Enums.PlayerState state)
    {
        switch (state)
        {
            case Enums.PlayerState.Observation:
                ObservationStateBehaviour.Enter();
                break;
            case Enums.PlayerState.Focus:
                FocusStateBehaviour.Enter();
                break;
        }
    }

    private void ExitState(Enums.PlayerState state)
    {
        switch (state)
        {
            case Enums.PlayerState.Observation:
                ObservationStateBehaviour.Exit();
                break;
            case Enums.PlayerState.Focus:
                FocusStateBehaviour.Exit();
                break;
        }
    }

    #endregion

    #region Teleportation

    private void UpdateRightHandTeleportation()
    {
        if (!isTeleportationEnabled || isTeleporting) return;

        // Checks if teleportation is valid?
        float dot = math.dot(-rightHandTeleportationRayInteractor.rayOriginTransform.up, Vector3.up);
        bool valid = dot > rightHandTeleportationDotTolerance;
        SetRightHandTeleportationRayEnabled(valid);

        if (valid) TryTeleportWithRightHand();
    }

    private void SetRightHandTeleportationRayEnabled(bool value)
    {
        if (rightHandTeleportationRayInteractor.gameObject.activeSelf == value) return;
        rightHandTeleportationRayInteractor.gameObject.SetActive(value);
    }

    private void TryTeleportWithRightHand()
    {
        if (rightHandTeleportationRayInteractor.hasHover)
        {
            if (rightHandTeleportationWaitTimer > rightHandTeleportationWaitDuration)
            {
                bool isHitValid = rightHandTeleportationRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);
                if (!isHitValid || (hit.transform.gameObject.layer != LayerMask.NameToLayer("Teleport"))) return;
                

                CustomTeleportationArea area = hit.transform.GetComponent<CustomTeleportationArea>();
                if (area == null)
                {
                    Debug.LogError("No component found.");
                    return;
                }
                Teleport(area.GetTeleportationPoint);
                rightHandTeleportationWaitTimer = 0;
            }
            else rightHandTeleportationWaitTimer += Time.deltaTime;
        }
        else rightHandTeleportationWaitTimer = 0;
    }

    public async void Teleport(Vector3 position)
    {
        if (!teleportationProvider.enabled)
        {
            Debug.LogError("Teleporter is not enabled");
            return;
        }

        var request = new TeleportRequest() { destinationPosition = position };
        var success = teleportationProvider.QueueTeleportRequest(request);
        if (!success)
        {
            Debug.LogError("Failed to teleport");
            return;
        }

        isTeleporting = true;
        await Task.Delay(1000);
        isTeleporting = false;
    }

    public float GetRightHandTeleportationWaitRatio => rightHandTeleportationWaitTimer / rightHandTeleportationWaitDuration;

    #endregion

    #region Input Actions Callbacks

    public void OnLeftJoystickAxis(InputAction.CallbackContext ctx)
    {
        switch (currentState)
        {
            case Enums.PlayerState.Observation:
                ObservationStateBehaviour.OnReceiveLeftJoystickAxis(ctx);
                break;
            case Enums.PlayerState.Focus:
                FocusStateBehaviour.OnReceiveLeftJoystickAxis(ctx);
                break;
        }
    }

    public void OnRightJoystickAxis(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Right joystick axis: {ctx.ReadValue<Vector2>()}");
    }

    public void OnTriggerButtonA(InputAction.CallbackContext ctx)
    {
        Debug.Log("Trigger Button A");
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
        Debug.Log("Right Pinch");
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
}