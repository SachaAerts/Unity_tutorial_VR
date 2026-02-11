using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportationComponent: MonoBehaviour
{
    [Header("Right Hand Teleportation")]
    [SerializeField] TeleportationProvider teleportationProvider;
    [SerializeField] private XRRayInteractor rightHandTeleportationRayInteractor;
    [SerializeField] private bool isTeleportationEnabled = true;
    [SerializeField] private float rightHandTeleportationDotTolerance = .9f;
    [SerializeField] private float rightHandTeleportationWaitDuration = 2f;

    private float rightHandTeleportationWaitTimer;
    private bool isTeleporting;

    public void SetTeleportationEnabled(bool value)
    {
        isTeleportationEnabled = value;
    }

    public void UpdateRightHandTeleportation()
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
}