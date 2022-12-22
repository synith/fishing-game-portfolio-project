using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodAnimation : MonoBehaviour
{
    [SerializeField] Animator fishingRodAnimator;

    public enum AnimationType { Cast, Struggle, }

    const string CAST_TRIGGER = "Cast";
    const string STRUGGLE_TRIGGER = "Struggle";


    string GetTriggerWord(AnimationType animationType) => animationType switch
    {
        AnimationType.Cast => CAST_TRIGGER,
        AnimationType.Struggle => STRUGGLE_TRIGGER,
        _ => STRUGGLE_TRIGGER,
    };

    public void PlayAnimation(AnimationType animationType)
    {
        fishingRodAnimator.SetTrigger(GetTriggerWord(animationType));
    }
}
