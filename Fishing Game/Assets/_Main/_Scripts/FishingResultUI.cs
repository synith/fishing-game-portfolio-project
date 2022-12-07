using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingResultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI difficultyText;

    private void Start()
    {
        FishingMechanic.OnFishAction += FishingMechanic_OnFishAction;
        FishingMechanic.OnFishingStart += FishingMechanic_OnFishingStart;
        FishingMechanic.OnFishingRestart += FishingMechanic_OnFishingRestart;
        FishingMechanic.OnDifficultyChanged += FishingMechanic_OnDifficultyChanged;
    }

    private void FishingMechanic_OnDifficultyChanged(object sender, FishingMechanic.Difficulty e)
    {
        difficultyText.SetText(e.ToString());
    }

    private void FishingMechanic_OnFishingRestart(object sender, EventArgs e)
    {
        resultText.SetText("Press T to test Fishing Mechanic");
    }

    private void FishingMechanic_OnFishingStart(object sender, EventArgs e)
    {
        resultText.SetText("Fishing...");
    }

    private void FishingMechanic_OnFishAction(object sender, bool e)
    {
        string result = e ? "Success!" : "Failed...";
        string restart = "Press R to Restart";
        resultText.SetText(result + "\n" + restart);
    }
}
