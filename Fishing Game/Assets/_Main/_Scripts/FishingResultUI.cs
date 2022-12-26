using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingResultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _resultText;

    private void OnEnable()
    {
        FishingMechanic.OnFishAttempt += FishingMechanic_OnFishAction;
        FishingMechanic.OnFishingStart += FishingMechanic_OnFishingStart;
        FishingMechanic.OnFishingReady += FishingMechanic_OnFishingRestart;
    }

    private void OnDisable()
    {
        FishingMechanic.OnFishAttempt -= FishingMechanic_OnFishAction;
        FishingMechanic.OnFishingStart -= FishingMechanic_OnFishingStart;
        FishingMechanic.OnFishingReady -= FishingMechanic_OnFishingRestart;
    }

    private void FishingMechanic_OnFishingRestart(object sender, EventArgs e)
    {
        _resultText.SetText("Press to Cast!");
    }

    private void FishingMechanic_OnFishingStart(object sender, EventArgs e)
    {
        _resultText.SetText("Fishing...");
    }

    private void FishingMechanic_OnFishAction(bool isAttemptPossible, bool isLastFishingDifficulty)
    {
        string status = isAttemptPossible ? isLastFishingDifficulty ? "Success!" : "Keep Going!" : "Failed";
        string recast = isAttemptPossible ? isLastFishingDifficulty? "Press to Recast" : "" : "Press to Recast";
        _resultText.SetText(status + "\n" + recast);
    }
}
