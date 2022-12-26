using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] Ring insideRing;
    [SerializeField] Ring outsideRing;
    [SerializeField] ZoneVisual zoneVisual;
    [SerializeField] SpriteRenderer zoneSpriteRenderer;
    [SerializeField] Color zoneColor;

    private void Awake()
    {
        zoneColor = zoneSpriteRenderer.color;
    }


    public float GetInsideRingRadius() => insideRing.GetRadius();
    public float GetOutsideRingRadius() => outsideRing.GetRadius();
    public void SetColor(Color color) => zoneSpriteRenderer.color = color;
    public Color GetColor() => zoneColor;

    public void SetZone(float radius, float thickness)
    {
        SetZoneRings(radius, thickness);
        UpdateZoneVisual(radius, thickness);
    }

    void SetZoneRings(float radius, float thickness)
    {
        insideRing.SetRadius(radius);
        outsideRing.SetRadius(radius + thickness);
    }

    void UpdateZoneVisual(float radius, float thickness) => zoneVisual.UpdateVisual(radius, thickness);
}
