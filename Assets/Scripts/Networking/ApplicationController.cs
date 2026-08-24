using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;

    [SerializeField] private HostSingleton hostPrefab;
    private async void Start() // can't call async methods without async, unity allows async start methods. 
    {
        DontDestroyOnLoad(gameObject);

        // servers don't have any graphics so no need to render anything
       await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    // method being called to calculate if a dedicated server or not
    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
        }
        else
        {
            // wait for authentication before going ot main menu
            // Go to main menu

            HostSingleton hostSingleton = Instantiate(hostPrefab);

            hostSingleton.CreateHost();

            // spawn in client singleton or host singleton if not a dedicated controller 
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient(); // any code after this line will have to wait for the createclient method to finish first 
           

            // if create client was successful 
            // route to main menu 
            if (authenticated)
            {
                // if it fails have them try again otherwise chage scene
                clientSingleton.GameManager.GoToMenu();

            }
        }
    }
}
