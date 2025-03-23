using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlaceSC : MonoBehaviour
{
    public GameObject hideText;
    public CreatureSC monster;
    public Transform monsterTransform;
    
    [Header("Hiding Place Settings")]
    public Transform hidingSpot; //To teleport the player into the hiding spot.
    public Transform pop; //To teleport the player out of the hiding spot.
    public CameraMovement cameraMovement; //To restrict the camera while hiding.
    
    private bool interactable, hiding;
    private PlayerController _player;
    
    public float loseDistance;

    private void Start()
    {
        interactable = false;
        hiding = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hideText.SetActive(true);
            interactable = true;
            _player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hideText.SetActive(false);
            interactable = false;
            _player = null;
        }
    }

    private void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.F) && !hiding)
        {
            hideText.SetActive(false);
            _player.transform.position = hidingSpot.position;
            float distance = Vector3.Distance(monsterTransform.position, _player.transform.position);
            if (distance > loseDistance)
            {
                if (monster.GetCurrentState() == CreatureSC.State.Chasing)
                {
                    monster.StopChasing();
                }
            }
            //stopHideText.SetActive(true);
            hiding = true;
            cameraMovement.isHiding = true;
            interactable = false;
            _player.Hide(true);
        }

        if (hiding && Input.GetKeyDown(KeyCode.F))
        {
            //stopHideText.SetActive(false);
            cameraMovement.isHiding = false;
            _player.transform.position = pop.position;
            hiding = false;
            _player.Hide(false);
        }
    }
}
