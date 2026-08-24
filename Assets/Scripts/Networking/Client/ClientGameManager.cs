using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager 
{
    private const string MenuSceneName = "Menu";
    // lasts an unkown amount of time can be there till task is done
    public async Task<bool> InitiAsync() // to know if whoever initalized this function was successful turn it into a bool task
    {
        // authenticate player
        // init unity services
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        // let whoever called gotomenu find out the end result
        SceneManager.LoadScene(MenuSceneName);
    }
}
