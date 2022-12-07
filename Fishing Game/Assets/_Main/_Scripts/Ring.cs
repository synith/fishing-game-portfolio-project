using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] float radius = 5f;
    [SerializeField] Color color = Color.white;

    public void SetRadius(float radius) => this.radius = radius;
    public float GetRadius() => radius;
}
