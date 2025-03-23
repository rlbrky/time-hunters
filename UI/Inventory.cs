using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public static InventoryItem carriedItem;

    [SerializeField] private GameObject canvas;
    [SerializeField] private InventorySlot[] inventorySlots;

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;
    
    [Header("Item List")]
    [SerializeField] private Item[] items;

    private void Awake()
    {
        instance = this;
        canvas.SetActive(false);
    }

    public void SpawnRandomItem(Item item = null)
    {
        Item _item = item;
        if (_item == null)
        {
            int random = Random.Range(0, items.Length);
            _item = items[random];
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //Check for empty slot
            if (inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            SpawnRandomItem();
        
        if (carriedItem == null) return;
        
        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null)
        {
            if (item.activeSlot.itemType != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }
        
        carriedItem = item;
        carriedItem._canvasGroup.blocksRaycasts = false; // Make the item interactable
        item.transform.SetParent(draggablesTransform);
    }

    /// <summary>
    /// This function also destroys item from the inventory, so no need to worry about that.
    /// </summary>
    /// <param name="neededItem">Required item type for use.</param>
    /// <returns></returns>
    public bool UseItemInInventory(ItemType neededItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //Check for the item in the inventory.
            if (inventorySlots[i].myItem != null && inventorySlots[i].myItem.activeSlot.itemType == neededItem)
            {
                Debug.Log("Found and used item.");
                //If it is the item we wanted to use, destroy it and apply changes.
                Destroy(inventorySlots[i].myItem.gameObject);
                return true;
            }
            else
            {
                Debug.LogWarning("item doesnt exist");
                return false;
            }
        }
        //Item couldn't be found.
        return false;
    }

    public void DestroyCarriedItem()
    {
        if (carriedItem != null)
        {
            Destroy(carriedItem.gameObject);
        }
    }
}
