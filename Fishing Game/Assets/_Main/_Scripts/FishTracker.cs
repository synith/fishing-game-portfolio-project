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

    List<FishSO> _availableFishList;
    List<FishSO> _caughtFishList;

    [SerializeField] FishListSO fishTier1;

    FishSO _activeFish;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        _caughtFishList = new List<FishSO>();
        _availableFishList = new List<FishSO>();

        foreach (FishSO fish in fishTier1.list)
        {
            _availableFishList.Add(fish);
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
        int index = Random.Range(0, _availableFishList.Count);
        FishSO fish = _availableFishList[index];

        return fish;
    }

    public void RecordFishCaught(FishSO fish)
    {
        if (IsAvailableFishListEmpty())
        {
            return;
        }

        Debug.Log($"Caught Fish: {fish}");
        _availableFishList.Remove(fish);
        _caughtFishList.Add(fish);

        if (IsAvailableFishListEmpty())
        {
            Debug.Log("YOU WIN!!!!");
            OnAllFishCaught?.Invoke();
            return;
        }

        foreach (var item in _availableFishList)
        {
            Debug.Log(item.name);
        }

        OnFishCaught?.Invoke(fish);
    }

    bool IsAvailableFishListEmpty() => _availableFishList.Count == 0;

    void SetActiveFish(FishSO fish)
    {
        _activeFish = fish;
        OnActiveFishChanged?.Invoke(fish);
    }

    public List<FishSO> GetAvailableFishList() => _availableFishList;
    public List<FishSO> GetCaughtFishList() => _caughtFishList;
    public FishSO GetActiveFish() => _activeFish;
}
