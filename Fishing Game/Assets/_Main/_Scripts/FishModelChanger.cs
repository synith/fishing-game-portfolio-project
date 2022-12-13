using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishModelChanger : MonoBehaviour
{
    [SerializeField] Transform position;
    Transform model;
    
    void OnEnable()
    {
        FishSO activeFish = FishTracker.Instance.GetActiveFish();
        model = Instantiate(activeFish.Model, position);
        FishTracker.Instance.SetRandomFishActive();
    }

    void OnDisable() => Destroy(model.gameObject);
}
