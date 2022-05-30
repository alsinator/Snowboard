using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MountainCatalog", menuName = "ScriptableObjects/MountainCatalog", order = 1)]
public class MountainCatalog : ScriptableObject
{
    [Header("Mountain bundle names")]
    [SerializeField] private List<MountainData> mountains;

    public int NumMountains => mountains.Count;

    public MountainData GetMountain(int mountainIdx)
    {
        return mountains[Mathf.Clamp(mountainIdx, 0, mountains.Count - 1)];
    }
}
