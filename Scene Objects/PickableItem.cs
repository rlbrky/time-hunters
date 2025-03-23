using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private Item item;
    
    private PlayerController _player;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.GetComponent<PlayerController>();
            _player.itemToPickUp = this;
            UI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.itemToPickUp = null;
            UI.SetActive(false);
        }
    }

    public void PickUpItem()
    {
        Inventory.instance.SpawnRandomItem(item);
        Destroy(gameObject);
    }
}
