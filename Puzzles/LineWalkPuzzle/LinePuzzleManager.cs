using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePuzzleManager : MonoBehaviour
{
    public static LinePuzzleManager Instance;

    [Header("Puzzle Variables")]
    [SerializeField] private int correctTile_TargetCount;
    
    private int correctTile_Count = 0;
    private List<LinePuzzleTile> _linePuzzleTiles;
    
    private void Awake()
    {
        Instance = this;
        _linePuzzleTiles = new List<LinePuzzleTile>();
    }

    public void UpdateTiles(bool isCorrect, LinePuzzleTile tile)
    {
        _linePuzzleTiles.Add(tile);
        
        if (isCorrect)
        {
            tile.LightTile();
            correctTile_Count++;
            Debug.Log("Stepped on a correct tile, current count: " + correctTile_Count);
        }
        else
        {
            tile.LightTile();
            StartCoroutine(ResetPuzzle());
        }
    }

    IEnumerator ResetPuzzle()
    {
        yield return new WaitForSeconds(1f);
        correctTile_Count = 0;
        foreach (LinePuzzleTile tile in _linePuzzleTiles)
            tile.ResetTile();
    }
}
