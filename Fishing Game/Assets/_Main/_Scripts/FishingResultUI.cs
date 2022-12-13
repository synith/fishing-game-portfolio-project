using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingResultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _resultText;
    [SerializeField] TextMeshProUGUI _difficultyText;

    private void OnEnable()
    {
        FishingMechanic.OnFishAttempt += FishingMechanic_OnFishAction;
        FishingMechanic.OnFishingStart += FishingMechanic_OnFishingStart;
        FishingMechanic.OnFishingRestart += FishingMechanic_OnFishingRestart;
        FishingMechanic.OnDifficultyChanged += FishingMechanic_OnDifficultyChanged;
    }
    private void OnDisable()
    {
        FishingMechanic.OnFishAttempt -= FishingMechanic_OnFishAction;
        FishingMechanic.OnFishingStart -= FishingMechanic_OnFishingStart;
        FishingMechanic.OnFishingRestart -= FishingMechanic_OnFishingRestart;
        FishingMechanic.OnDifficultyChanged -= FishingMechanic_OnDifficultyChanged;
    }

    private void FishingMechanic_OnDifficultyChanged(object sender, FishingMechanic.Difficulty e)
    {
        _difficultyText.SetText(e.ToString());
    }

    private void FishingMechanic_OnFishingRestart(object sender, EventArgs e)
    {
        _resultText.SetText("Press T to Cast");
    }

    private void FishingMechanic_OnFishingStart(object sender, EventArgs e)
    {
        _resultText.SetText("Fishing...");
    }

    private void FishingMechanic_OnFishAction(bool isAttemptPossible, bool isLastFishingDifficulty)
    {
        string result = isAttemptPossible ? isLastFishingDifficulty ? "Success!" : "Keep Going!" : "Failed";
        string restart = isAttemptPossible ? isLastFishingDifficulty? "Press R to Restart" : "" : "Press R to Restart";
        _resultText.SetText(result + "\n" + restart);
    }
}
