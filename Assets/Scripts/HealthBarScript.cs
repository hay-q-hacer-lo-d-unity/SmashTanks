using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private Image fillImage;   // assign in prefab Inspector

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // ✅ Billboard effect (always face the camera)
        if (mainCamera)
            transform.forward = mainCamera.transform.forward;
    }

    public void SetHealth(float current, float max)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }
}
