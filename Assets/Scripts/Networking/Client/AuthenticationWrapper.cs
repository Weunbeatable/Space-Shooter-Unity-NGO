using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper 
{
    public static AuthState authState { get; private set; } = AuthState.NotAuthenticated;

    // need to ask ugs to authetnicate this, tatkes longer than a frame to authenticate so its better to make it async
    public static async Task<AuthState> DoAuth(int maxTries = 5) // after 5 tries return
    {
        if (authState == AuthState.Authenticated)
        {
            return authState;
        }

        // what if  asecond part of code tries to access auth state while authentication happens
        if(authState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!"); // not a problem just waiting for authentication to complete first 
            await Authenticating();
            return authState;
        }

        await SignInAnonymously(maxTries);
        return authState;
    }


    private static async Task<AuthState> Authenticating()
    {
        // check if still authenticating or not, wait for state change until a failure where we stop authenticating
        while(authState == AuthState.Authenticating || authState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return authState;
    }
    private static async Task SignInAnonymously(int maxTries)
    {
        authState = AuthState.Authenticating;
        int tries = 0;
        while (authState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // anonymous means they don't need to do anytning else 
                                                                               // to authenticate like google, facebook etc they can just authenticate without anything. (good for mobile & testing) 

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    // things went well
                    authState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException authException) // fails to authenticate
            {
                // errors expected
                Debug.LogError(authException);
                authState = AuthState.Error;

            }
            catch(RequestFailedException requestException) // e.g. requests fail to reach server no internet etc
            {
                Debug.LogError(requestException);
                authState = AuthState.Error;
            }
            
            tries++;
            // break out so it doesn't immediately happen again
            await Task.Delay(1000);
        }
        // once all tries are done
        if(authState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {tries} attempts.");
            authState = AuthState.Timeout; // no errors but wasn't able to authenticate successfully
        }
      
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