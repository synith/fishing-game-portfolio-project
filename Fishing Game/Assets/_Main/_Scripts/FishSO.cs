using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Fish")]
public class FishSO : ScriptableObject
{
    public Transform Model;
    public List<FishingMechanic.Difficulty> difficultyList;
}
