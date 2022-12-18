using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishLabelUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    FishSO _fish;

    [SerializeField] Color _availableColor;
    [SerializeField] Color _caughtColor;

    void Awake()
    {
        SetTextColor(_availableColor);
    }

    void OnEnable()
    {
        FishTracker.Instance.OnFishCaught += FishTracker_OnFishCaught;
    }
    void OnDisable()
    {
        FishTracker.Instance.OnFishCaught -= FishTracker_OnFishCaught;
    }

    void FishTracker_OnFishCaught(FishSO fish)
    {
        if (fish != _fish) return;

        SetTextColor(_caughtColor);
    }

    public void SetFish(FishSO fish)
    {
        _fish = fish;

        SetText(fish.Name);
    }

    void SetText(string text) => _text.SetText(text);
    void SetTextColor(Color color) => _text.color = color;
}
