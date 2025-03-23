using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { PuzzlePiece, Needs}

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public ItemType itemTag;
    
    public GameObject prefab;
}
