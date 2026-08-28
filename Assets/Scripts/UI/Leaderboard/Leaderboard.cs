using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Assets.Scripts.UI.Leaderboard;
using System;
using System.Linq;
public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityDisplay;
    // int to specify how many entities can be seen at once, if there are more than max it will just show players ranking even if not in top set
    [SerializeField] private int entitiesToDisplay = 8;
    // will need to pass a custom type into here to be able to pass objects with things like coins leaderboards tc
    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    // store entities spawned in a list
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>(); // not a network so initalizable here

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
        ShipPlayer.OnPlayerDespawned += HandlePlayerDespawned;
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
            ShipPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        // change event will allow me to know if it was added removed etc basically crud operations

        // check eventtypes
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                // lambda expression to check for any entity displays where client id == change event client id
                // spawn new one (empty displays) if there aren't any displays that match clientId
                if(!entityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    // when added to list spawn in ui entry when added to leaderboard
                    // add entity to list
                   LeaderboardEntityDisplay leaderboardEntity = Instantiate(
                       leaderboardEntityDisplay, leaderboardEntityHolder); // need to move to list to handle specific instance
                    leaderboardEntity.Initalise(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.playerName,
                        changeEvent.Value.Coins);
                    entityDisplays.Add(leaderboardEntity); // when a new one is spawned in and the display check is done, wont make duplicate displays
                }
            
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                 LeaderboardEntityDisplay displayToRemove =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId); // find first occurence null if none
                if (displayToRemove != null)
                {
                    // set parent to null before destroying
                    displayToRemove.transform.SetParent(null);
                    Destroy(displayToRemove.gameObject);
                    entityDisplays.Remove(displayToRemove);
                }

                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayToUpdate =
                    entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        // lambda expression to sort so larger coins at top of leaderboard and smaller coin coints below
        // largest to smallest
        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        // update UI for same order
        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            bool shouldShow = i <= entitiesToDisplay - 1; // max entities  - 1 since array starts at 0
            entityDisplays[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
        }
        // if the player has a low score and doesn't reach leaderboard. just show them at the bottom
        // of the leaderboard. remove last person and put  username in that place 
        LeaderboardEntityDisplay myDisplay =
            entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        // ensure usally exists
        if (myDisplay != null)
        {
            // if current position is past leaderboard, hide the person in the last leaderbord position and replace them
            // with the user. 
            if (myDisplay.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitiesToDisplay -1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
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
        player.Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
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
        player.Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) =>
            HandleCoinsChanged(player.OwnerClientId, newCoins);
    }
    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        // go through entity states synced to client find matching player state and update coin value, then update all
        // client leaderboard values
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) { continue; }
            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = leaderboardEntities[i].ClientId,
                playerName = leaderboardEntities[i].playerName,
                Coins = newCoins
            };
              
        }
    }
}
