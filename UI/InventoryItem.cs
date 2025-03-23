using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    //To block raycast while we are carrying it.
    [SerializeField] private Image _itemIcon; //Filled through inspector because if the player doesn't use inventory, it will cause an error.
    public CanvasGroup _canvasGroup {get; private set;}
    
    public Item myItem {get; set;}
    public InventorySlot activeSlot {get; set;}

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _itemIcon = GetComponent<Image>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        _itemIcon.sprite = item.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.instance.SetCarriedItem(this);
        }
    }
}
