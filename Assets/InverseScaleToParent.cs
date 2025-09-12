using UnityEngine;

[ExecuteAlways]
public class InverseScaleToParent : MonoBehaviour
{
    void LateUpdate()
    {
        var p = transform.parent;
        if (!p) return;
        var s = p.lossyScale;
        transform.localScale = new Vector3(
            Mathf.Approximately(s.x, 0f) ? 1f : 1f / s.x,
            Mathf.Approximately(s.y, 0f) ? 1f : 1f / s.y,
            Mathf.Approximately(s.z, 0f) ? 1f : 1f / s.z
        );
    }
}