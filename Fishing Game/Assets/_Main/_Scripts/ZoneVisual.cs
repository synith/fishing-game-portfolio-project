using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneVisual : MonoBehaviour
{
    [SerializeField] Transform maskTransform;
    [SerializeField] Transform circleTransform;

    const float RADIUS_TO_SCALE = 2f;

    void SetCircleScale(float thickness)
        => circleTransform.localScale = maskTransform.localScale + RADIUS_TO_SCALE * thickness * Vector3.one;
    void SetMaskScale(float radius)
        => maskTransform.localScale = RADIUS_TO_SCALE * radius * Vector3.one;

    public void UpdateVisual(float radius, float thickness)
    {
        SetMaskScale(radius);
        SetCircleScale(thickness);
    }
}
