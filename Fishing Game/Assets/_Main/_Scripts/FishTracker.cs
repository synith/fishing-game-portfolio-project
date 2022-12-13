using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishTracker : MonoBehaviour
{
    public static FishTracker Instance { get; private set; }

    public event Action<FishSO> OnActiveFishChanged;

    List<FishSO> _availableFishList;
    List<FishSO> _caughtFishList;

    [SerializeField] FishListSO fishTier1;

    public FishSO ActiveFish { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _availableFishList = fishTier1.list;
        SetRandomFishActive();
    }


    public void SetRandomFishActive()
    {
        FishSO randomFish = GetRandomAvailableFish();
        SetActiveFish(randomFish);
    }

    FishSO GetRandomAvailableFish()
    {
        int index = Random.Range(0, _availableFishList.Count);
        FishSO fish = _availableFishList[index];        

        return fish;
    }

    void RecordFishCaught(FishSO fish)
    {
        _availableFishList.Remove(fish);
        _caughtFishList.Add(fish);
    }

    void SetActiveFish(FishSO fish)
    {
        ActiveFish = fish;
        OnActiveFishChanged?.Invoke(fish);
    }

    FishSO GetActiveFish() => ActiveFish;
}
