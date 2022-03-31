using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsTracker : MonoBehaviour
{
    public GameObject[] prefabsSetList;
    private static GameObject[] prefabsArray;


    private void Awake()
    {
        prefabsArray = prefabsSetList;
    }

    public static GameObject GetPrefab(int prefabIndex)
    {
        return prefabsArray[prefabIndex];
    }
}
