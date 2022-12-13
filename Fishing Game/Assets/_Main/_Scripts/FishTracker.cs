using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishTracker : MonoBehaviour
{
    public static FishTracker Instance { get; private set; }

    public event Action<FishSO> OnActiveFishChanged;

    List<FishSO> availableFishList;
    List<FishSO> caughtFishList;

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

        availableFishList = fishTier1.list;
        SetRandomFishActive();
    }


    public void SetRandomFishActive()
    {
        FishSO randomFish = GetRandomAvailableFish();
        SetActiveFish(randomFish);
    }

    FishSO GetRandomAvailableFish()
    {
        int index = Random.Range(0, availableFishList.Count);
        FishSO fish = availableFishList[index];        

        return fish;
    }

    void RecordFishCaught(FishSO fish)
    {
        availableFishList.Remove(fish);
        caughtFishList.Add(fish);
    }

    void SetActiveFish(FishSO fish)
    {
        ActiveFish = fish;
        OnActiveFishChanged?.Invoke(fish);
    }

    FishSO GetActiveFish() => ActiveFish;
}
