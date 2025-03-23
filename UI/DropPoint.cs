using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPoint : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerController player;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.carriedItem != null)
            {
                Instantiate(Inventory.carriedItem.myItem.prefab, player.itemCarryPos.position, Quaternion.identity);
                Inventory.instance.DestroyCarriedItem();
            }
        }
    }
}
