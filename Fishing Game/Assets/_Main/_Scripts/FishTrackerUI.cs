using System.Collections.Generic;
using UnityEngine;

public class FishTrackerUI : MonoBehaviour
{
    [SerializeField] RectTransform _fishLabelPrefab;
    [SerializeField] RectTransform _fishLabelContainer;

    void Start()
    {
        List<FishSO> availableFish = FishTracker.Instance.GetAvailableFishList();

        foreach (FishSO fish in availableFish)
        {
            RectTransform fishLabelGameObject = Instantiate(_fishLabelPrefab, _fishLabelContainer);
            FishLabelUI fishLabel = fishLabelGameObject.GetComponent<FishLabelUI>();

            fishLabel.SetFish(fish);
        }
    }
}