using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CustomTeleportationArea : MonoBehaviour
{
    [SerializeField] private Transform reference;
    [SerializeField] private Image fillImage;
    [SerializeField] private SpriteRenderer circle;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color unhoverColor;

    Vector3 defaultSize;
    bool isHover;
    
    const float speed = 5;
    const float amplitude = .1f;

    public Vector3 GetTeleportationPoint => reference.position;

    private void Start()
    {
        defaultSize = circle.transform.localScale;
        OnUnhover();
    }

    private void Update()
    {
        if (!isHover) return;
        circle.transform.localScale = defaultSize + math.sin(Time.time * speed) * amplitude * Vector3.one;
        fillImage.fillAmount = PlayerController.Singleton.GetRightHandTeleportationWaitRatio;
    }

    public void OnHover()
    {
        isHover = true;
        circle.color = hoverColor;
    }

    public void OnUnhover()
    {
        isHover = false;
        fillImage.fillAmount = 0;
        circle.color = unhoverColor;
    }
}