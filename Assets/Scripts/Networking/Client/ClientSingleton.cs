using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager GameManager {  get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindFirstObjectByType<ClientSingleton>();
            if (instance == null)
            {
                Debug.LogError("No ClientSingleton in the scene!");
                return null;
            }
            return instance;
        }
       
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject); // exist even when scenes change
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        // without awaiting code after this method wont wait for it to finish. Needed so authentication happens before anything else
        return await GameManager.InitiAsync();
    }

}
