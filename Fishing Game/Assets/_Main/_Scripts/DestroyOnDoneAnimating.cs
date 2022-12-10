using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDoneAnimating : MonoBehaviour
{
    [SerializeField]float delay = 0f;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }
}
