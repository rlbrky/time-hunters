using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPuzzle : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] private Transform batteryPlacementLocation;
    [SerializeField] private Canvas UI;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private int neededBatteryCount;

    private bool canUse;
    private int placedBatteryCount = 0;

    private void Update()
    {
        if(canUse && Input.GetKeyDown(KeyCode.F))
            AddBattery();
    }

    public void AddBattery() //Either take it from inventory directly. Or make them drop and drag it to the collider.
    {
        if (Inventory.instance.UseItemInInventory(ItemType.PuzzlePiece))
        {
            Instantiate(batteryPrefab, batteryPlacementLocation.position, Quaternion.identity);
            placedBatteryCount++;
            if (placedBatteryCount >= neededBatteryCount)
            {
                Debug.Log("Puzzle Complete.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI.gameObject.SetActive(true);
            canUse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI.gameObject.SetActive(false);
            canUse = false;
        }
    }
}