using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    // location of lobbyitems
    [SerializeField] private Transform lobbyItemParent;
    // populate list of lobbies
    [SerializeField] private LobbyItem lobbyItemPrefab;
    // prevent scenarios like usuer spamming join and joining multiple times
    private bool isJoining;
    private bool isRefreshing;
    private void OnEnable()
    {
        RefreshList(); 
    }

    public async void RefreshList()
    {
        if (isRefreshing) { return; }
        
        isRefreshing = true;


        try
        {
            // get list of loibbies, in this case get top 25 lobbies that meet this filter criteria 
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25; // how many lobbies to grab, for large games you would want to get them in batches for pagination (pages)

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                // essentially code above is saying check available slots in lobby
                // ensure that they are greater than
                // 0
                // make sure there is room in lobby to show in list
                new QueryFilter(QueryFilter.FieldOptions.IsLocked,
                op: QueryFilter.OpOptions.EQ,
                value: "0")
            };

            // after setting up filter for lobbies, retrieve them
          QueryResponse lobbies =  await Lobbies.Instance.QueryLobbiesAsync(options); // returns an object of query responses
            // go over any buttons already spawned in and destory them and then spawn in any new lobbies
            foreach (Transform child in lobbyItemParent) // remove curently existing lobby items
            {
                Destroy(child.gameObject);
            }
            foreach (Lobby lobby in lobbies.Results) // spawn in new ones
            {
               LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initalise(this, lobby); // takes the lobby items from the loop 
            }    
         
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
      

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) {return; }
        isJoining = true;
        // code calling this can't wait for this to finish asyncrohnously but it can still call async
        try
        {
           // lobby passed in and one returning (joining lobby are different joining will have the code to join the allocation. 
           Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            // read join code form lobby
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
        isJoining=false;
    }
}
