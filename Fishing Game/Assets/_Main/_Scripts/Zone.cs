using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] Ring insideRing;
    [SerializeField] Ring outsideRing;
    [SerializeField] ZoneVisual zoneVisual;

    [SerializeField, Range(0.1f, 6f)] float radius = 1f;
    [SerializeField, Range(0.1f, 3f)] float thickness = 1f;

    [SerializeField] SpriteRenderer zoneSpriteRenderer;


    public float GetInnerRadius() => insideRing.GetRadius();
    public float GetOuterRadius() => outsideRing.GetRadius();
    public void SetColor(Color color) => zoneSpriteRenderer.color = color;

    public void SetZone(float radius, float thickness)
    {
        this.radius = radius;
        this.thickness = thickness;        
    }

    public void UpdateZone(float radius, float thickness)
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
