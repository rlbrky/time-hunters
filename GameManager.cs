using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Player Prefab")]
    [SerializeField] GameObject playerPrefab;
    
    [Header("Time Objects")]
    [SerializeField] GameObject past;
    [SerializeField] GameObject future;
    
    [Header("Spawn Points")]
    [SerializeField] Transform spawnPoint1;
    [SerializeField] Transform spawnPoint2;

    private int spawnedPlayerCount = 0;
    private bool isPast = true;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnServerInitialized()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Destroy(gameObject);
            return;
        }

    }

    public void ChangeTime()
    {
        if (isPast)
        {
            past.SetActive(false);
            future.SetActive(true);
            isPast = false;
        }
        else
        {
            past.SetActive(true);
            future.SetActive(false);
            isPast = true;
        }
    }

    private void OnClientConnected(ulong obj)
    {
        Debug.Log("Client connected, clientId= " + obj);

        // Spawn client
        HandleSpawnServerRpc(obj);

        // Start game
        if (spawnedPlayerCount == 2)
        {
            Debug.Log("Both players connected - game starting!");
        }
    }
    
    [ServerRpc]
    public void HandleSpawnServerRpc(ulong clientId)
    {
        spawnedPlayerCount++;
        NetworkObject newNetworkObject;
        switch (spawnedPlayerCount)
        {
            case 1:
                newNetworkObject = Instantiate(playerPrefab, spawnPoint1.position, Quaternion.identity).GetComponent<NetworkObject>();
                newNetworkObject.SpawnAsPlayerObject(clientId, true);
                newNetworkObject.transform.position = spawnPoint1.position;
                break;
            
            case 2:
                newNetworkObject = Instantiate(playerPrefab, spawnPoint2.position, Quaternion.identity).GetComponent<NetworkObject>();
                newNetworkObject.SpawnAsPlayerObject(clientId, true);
                newNetworkObject.transform.position = spawnPoint2.position;
                break;
        }
    }
}
