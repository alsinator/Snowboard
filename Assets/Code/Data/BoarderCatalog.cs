using Snowboard.Views;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BoarderCatalog", menuName = "ScriptableObjects/BoarderCatalog", order = 1)]
public class BoarderCatalog : ScriptableObject
{

    [SerializeField] private BoarderView boarder;

    [SerializeField] private List<Material> boarderMaterials;


    public BoarderView BoarderPrefab => boarder;

    public int NumBoarders => boarderMaterials.Count;

    public Material GetBoarderMaterial(int boarderIdx)
    {
        return boarderMaterials[Mathf.Clamp(boarderIdx, 0, NumBoarders - 1)];
    }
}
