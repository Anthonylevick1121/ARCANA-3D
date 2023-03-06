using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public struct PotionPuzzleCategory
{
    // template: every component on this will be applied to every bottle
    // prefabs: the available bottle types, will be referenced by index
    // location parent: the children of this game object will be used to define the possible spawn locations.
    // [SerializeField] private GameObject template;
    [SerializeField] private PotionPuzzleObject[] prefabs;
    [SerializeField] private Transform locationParent;
    [SerializeField] private GameObject specialEffect; // particle effect to apply to one object.
    
    // the cached start locations of each object during this instance of the game
    private Transform[] startLocations;
    private int specialEffectIndex;
    
    private string name;
    
    public void ValidatePrefabs(int targetCount, string name)
    {
        this.name = name;
        Assert.IsTrue(prefabs.Length == targetCount,
            "list of "+name+" prefabs must be size "+targetCount);
    }
    
    public void InitializePlacement(int specialEffectIndex = -1)
    {
        this.specialEffectIndex = specialEffectIndex; // save it
        // construct location array
        List<int> startLocationList = new ();
        int numChildren = locationParent.childCount;
        for (int i = 0; i < numChildren; i++) startLocationList.Add(i);
        // randomize
        startLocations = new Transform[startLocationList.Count];
        for (int i = 0; i < startLocations.Length; i++)
        {
            int idx = Random.Range(0, startLocationList.Count);
            startLocations[i] = locationParent.GetChild(startLocationList[idx]);
            startLocationList.RemoveAt(idx);
        }
        
        // create and track the objects
        // activeObjects = new GameObject[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            SpawnObject(i);
        }
    }
    
    private void SpawnObject(int id)
    {
        PotionPuzzleObject obj = Object.Instantiate(prefabs[id], startLocations[id].position, startLocations[id].rotation);
        obj.Id = id;
        if (id == specialEffectIndex)
            Object.Instantiate(specialEffect, obj.transform); // add the special effect
    }
    
    public void ReplaceConsumable(PotionPuzzleObject obj)
    {
        int idx = obj.Id;
        Object.Destroy(obj.gameObject);
        SpawnObject(idx);
    }
    // public GameObject GetObject(int index) => activeObjects[index];
}
