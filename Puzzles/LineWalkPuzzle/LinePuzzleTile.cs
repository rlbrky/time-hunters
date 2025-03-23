using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePuzzleTile : MonoBehaviour
{
    [SerializeField] private bool isCorrectTile;
    [SerializeField] private Renderer _renderer;
    
    private Color _originalColor;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision enter, " + other.gameObject.name);
        
        if(other.CompareTag("Player"))
            LinePuzzleManager.Instance.UpdateTiles(isCorrectTile, this);
    }

    public void LightTile()
    {
        if (isCorrectTile)
        {
            _renderer.material.color = Color.green;
        }
        else
        {
            _renderer.material.color = Color.red;
        }
    }

    public void ResetTile()
    {
        _renderer.material.color = _originalColor;
    }
}
