using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Assets.Scripts.UI.Leaderboard;
using System;
public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityDisplay;

    // will need to pass a custom type into here to be able to pass objects with things like coins leaderboards tc
    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged; // network sync to client. 
            // for anyone already in server add them to leaderboard, avoid duplicates
            foreach (LeaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }


        // will allow server keeping track of all players and update list on spawn and despawn
        if (IsServer) { 
        ShipPlayer[] players = FindObjectsByType<ShipPlayer>(FindObjectsSortMode.None);
        foreach (ShipPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        ShipPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        ShipPlayer.OnPlayerSpawned += HandlePlayerDespawned;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged; // network sync to client. 
        }
            
        
        if (IsServer)
        {
            ShipPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            ShipPlayer.OnPlayerSpawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        // change event will allow me to know if it was added removed etc basically crud operations

        // check eventtypes
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                // when added to list spawn in ui entry when added to leaderboard
                Instantiate(leaderboardEntityDisplay, leaderboardEntityHolder); // need to move to list to handle specific instance
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                break;
        }
    }

    // add and remove from server on spawn and despawn
    private void HandlePlayerSpawned(ShipPlayer player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            playerName  = player.PlayerName.Value,
            Coins = 0 // value they start off with 
        });
    }

    private void HandlePlayerDespawned(ShipPlayer player)
    {
       
        // on despawn need to cycle through entities and find matching player first before deleting them 
        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) { continue; }

            // if found
            leaderboardEntities.Remove(entity);
        
            break;
        }
    }
}
