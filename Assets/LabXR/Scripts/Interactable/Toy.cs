using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class Toy : MonoBehaviour
{   
    public Rigidbody rigidbodyToy;
    public bool IsTargeted = false;
    public bool IsSelected = false;

    void Start()
    {
        rigidbodyToy = GetComponent<Rigidbody>();
    }
    
    public void OnHoverEnterToy(HoverEnterEventArgs args)
        => IsTargeted = true;

    public void OnHoverExitedToy(HoverExitEventArgs args)
        => IsTargeted = false;      
}