using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodAnimation : MonoBehaviour
{
    [SerializeField] Animator fishingRodAnimator;

    const string CAST_TRIGGER = "Cast";

    public void PlayCastAnimation() => fishingRodAnimator.SetTrigger("Cast");
}
