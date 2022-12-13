using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObject/Fish List")]
public class FishListSO : ScriptableObject
{
    public List<FishSO> list;
}
