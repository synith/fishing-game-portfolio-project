using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishTracker : MonoBehaviour
{
    public static FishTracker Instance { get; private set; }

    public event Action<FishSO> OnActiveFishChanged;
    public event Action OnAllFishCaught;
    public event Action<FishSO> OnFishCaught;

    List<FishSO> availableFishList;
    List<FishSO> caughtFishList;

    [SerializeField] FishListSO fishTier1;

    FishSO activeFish;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        caughtFishList = new List<FishSO>();
        availableFishList = new List<FishSO>();

        foreach (FishSO fish in fishTier1.list)
        {
            availableFishList.Add(fish);
        }
        SetRandomFishActive();
    }


    public void SetRandomFishActive()
    {
        if (IsAvailableFishListEmpty()) return;

        FishSO randomFish = GetRandomAvailableFish();
        SetActiveFish(randomFish);
    }

    FishSO GetRandomAvailableFish()
    {
        int index = Random.Range(0, availableFishList.Count);
        FishSO fish = availableFishList[index];

        return fish;
    }

    public void RecordFishCaught(FishSO fish)
    {
        if (IsAvailableFishListEmpty())
        {
            return;
        }

        Debug.Log($"Caught Fish: {fish}");
        availableFishList.Remove(fish);
        caughtFishList.Add(fish);
        OnFishCaught?.Invoke(fish);

        if (IsAvailableFishListEmpty())
        {
            Debug.Log("YOU WIN!!!!");
            OnAllFishCaught?.Invoke();
            return;
        }        
    }

    bool IsAvailableFishListEmpty() => availableFishList.Count == 0;

    void SetActiveFish(FishSO fish)
    {
        activeFish = fish;
        OnActiveFishChanged?.Invoke(fish);
    }

    public List<FishSO> GetAvailableFishList() => availableFishList;
    public List<FishSO> GetCaughtFishList() => caughtFishList;
    public FishSO GetActiveFish() => activeFish;
}
