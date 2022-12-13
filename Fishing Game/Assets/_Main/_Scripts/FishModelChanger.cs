using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishModelChanger : MonoBehaviour
{
    [SerializeField] Transform position;
    Transform model;
    
    void OnEnable()
    {
        FishSO activeFish = FishTracker.Instance.ActiveFish;
        model = Instantiate(activeFish.Model, position);
    }

    void OnDisable() => Destroy(model.gameObject);
}
