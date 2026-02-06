using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Recenter Component allows the player to reset their rotation by putting their hands up for a certain time
/// </summary>
public class RecenterComponent : MonoBehaviour
{
    [SerializeField] private GameObject recenterUI;
    [SerializeField] private Image fillImage;
    [SerializeField] private Transform leftHand, rightHand, head;
    [SerializeField] private bool isRecenterEnabled = true;
    [SerializeField] private float recenterHandsHeight = 1;
    [SerializeField] private float recenterWaitDuration = 1;
    [SerializeField] private float recenterCooldownDuration = 2;
    private float recenterWaitTimer;

    public void Init()
    {
        recenterUI.SetActive(false);
    }

    public void UpdateRecenter()
    {
        if (!isRecenterEnabled) return;

        float leftHandHeight = leftHand.position.y - head.position.y;
        float rightHandHeight = rightHand.position.y - head.position.y;

        bool valid = leftHandHeight > recenterHandsHeight && rightHandHeight > recenterHandsHeight;
        if (valid) TryRecenter();
        else
        {
            if (recenterWaitTimer != 0)
            {
                recenterWaitTimer = 0;
                fillImage.fillAmount = 0;
                recenterUI.SetActive(false);
            }
        }
    }

    private void TryRecenter()
    {
        if (recenterWaitTimer > recenterWaitDuration)
        {
            RecenterPlayer();
            recenterWaitTimer = 0;
            fillImage.fillAmount = 0;
            recenterUI.SetActive(false);
        }
        else
        {
            if (recenterWaitTimer == 0) recenterUI.SetActive(true);
            fillImage.fillAmount = GetRecenterRatio;
            recenterWaitTimer += Time.deltaTime;
        }
    }

    public void RecenterPlayer()
    {
        float newAngle = head.eulerAngles.y;
        Quaternion newRotation = Quaternion.Euler(0, newAngle, 0);
        transform.rotation = newRotation;
        head.localRotation = Quaternion.identity;
        StartCooldown();
    }

    private async void StartCooldown()
    {
        isRecenterEnabled = false;
        await Task.Delay((int)(recenterCooldownDuration * 1000));
        isRecenterEnabled = true;
    }

    public float GetRecenterRatio => recenterWaitTimer / recenterWaitDuration;
}