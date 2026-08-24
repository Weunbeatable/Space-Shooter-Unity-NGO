using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class AuthenticationWrapper 
{
    public static AuthState authState { get; private set; } = AuthState.NotAuthenticated;

    // need to ask ugs to authetnicate this, tatkes longer than a frame to authenticate so its better to make it async
    public static async Task<AuthState> DoAuth(int maxTries = 5) // after 5 tries return
    {
        if(authState == AuthState.Authenticated)
        {
            return authState;
        }
        authState = AuthState.Authenticating;

        int tries = 0;
        while(authState == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); // anonymous means they don't need to do anytning else 
            // to authenticate like google, facebook etc they can just authenticate without anything. (good for mobile & testing) 

            if(AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                // things went well
                authState = AuthState.Authenticated;
                break;
            }
            tries++;
            // break out so it doesn't immediately happen again
            await Task.Delay(1000);
        }
        return authState;
    }
}
// enum for state of authentiation
public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}