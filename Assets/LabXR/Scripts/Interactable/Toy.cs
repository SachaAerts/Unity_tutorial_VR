using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class Toy : MonoBehaviour
{
    [SerializeField] private Transform leftControllerPlayer;
    
    [Header("Distance Settings")]
    [SerializeField] private float fixedDistance = 1f;
    
    private Rigidbody rigidbodyToy;
    private bool isTargeted = false;
    private bool isSelected = false;

    void Start()
    {
        rigidbodyToy = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isSelected)
        {
            FollowPlayer();
        }
    }
    
    public void OnHoverEnterToy(HoverEnterEventArgs args)
        => isTargeted = true;

    public void OnHoverExitedToy(HoverExitEventArgs args)
        => isTargeted = false;

    public void OnSelectEnteredToy(SelectEnterEventArgs args)
    {  
        if (isTargeted)
        {
            isSelected = true;
            fixedDistance = Vector3.Distance(transform.position, leftControllerPlayer.position);
            rigidbodyToy.isKinematic = true;

            PlayerController.Singleton.FocusStateBehaviour.SetFocusTransform(transform);
            PlayerController.Singleton.SetState(Enums.PlayerState.Focus);
        }   
    } 
      

    public void OnSelectExitedToy(SelectExitEventArgs args)
    {
        isSelected = false;
        rigidbodyToy.isKinematic = false;
        PlayerController.Singleton.SetState(Enums.PlayerState.Observation);
    }

    private void FollowPlayer()
    {
        transform.position = leftControllerPlayer.position + leftControllerPlayer.forward * fixedDistance;
    }
}