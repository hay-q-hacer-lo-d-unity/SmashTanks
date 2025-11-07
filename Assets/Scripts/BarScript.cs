using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    
    

    private void LateUpdate()
    {
        if (_mainCamera) transform.forward = _mainCamera.transform.forward;
    }

    public void Set(float current, float max)
    {
        if (fillImage) fillImage.fillAmount = Mathf.Clamp01(current / max);
    }
    
    public void DestroyBar()
    {
        Destroy(gameObject);
    }
}
