using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMenu : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject HowToPlayMenu;
    [SerializeField] GameObject SettingsMenu;
    // Start is called before the first frame update

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {  return; }
        inputReader.OnMenuEvent += OpenMainMenu;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.OnMenuEvent -= OpenMainMenu;
    }
    public void LeaveGame()
    {

        if (NetworkManager.Singleton.IsHost)
        {
            // this will handle shutting down the server as a host
            HostSingleton.Instance.gameManager.Shutdown();

            //but if you're a client you need a way to leave, also hosts are types of clients
            // so they need to be able to safely leave as well. 
        }

        // case for when player wants to leave and hits leave button 
        ClientSingleton.Instance.GameManager.Disconnect();
    }
    public void OpenMainMenu()
    {
        if (!IsOwner) { return; }
        MainMenu.SetActive(true);
        DisablePlayerControls();
    }

    #region UI Navigation
    public void ResumeGame()
    {
        if (!IsOwner) { return; }
        MainMenu.SetActive(false);
        EnablePlayerControls();
    }

    public void OpenControlsPanel(GameObject PanelToOpen, List<GameObject> PanelsToClose, GameObject[] SubPanels)
    {
        if (!IsOwner) { return; }
        foreach (GameObject panel in PanelsToClose)
        {
            panel.SetActive(false);
        }
        PanelToOpen.SetActive(true);
        SubPanels[0].SetActive(true);
        for (int i = 1; i<= SubPanels.Length; i++)
        {
            SubPanels[i].SetActive(false); // This way it always starts on the 0th index and I don't 
        }
    }

    public void BackButton(GameObject currentPanelToClose)
    {
        if (!IsOwner) { return; }
        currentPanelToClose.SetActive(false);
        MainMenu.SetActive(true);
    }
    
        
    #endregion
    #region PlayerControls
    private void DisablePlayerControls()
    {
        if (!this.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement)) { return; }
        {
            playerMovement.enabled = false;
        }
        if (!this.TryGetComponent<ThirdPersonControls>(out ThirdPersonControls thirdPersonControls)) { return; }
        {
            thirdPersonControls.enabled = false;
        }
        if (!this.TryGetComponent<PlayerAiming>(out PlayerAiming playerAiming)) { return; }
        {
            playerAiming.enabled = false;
        }
        if (!this.TryGetComponent<ProjectileLauncher>(out ProjectileLauncher projectileLauncher)) { return; }
        {
            projectileLauncher.enabled = false;
        }
        if (!this.TryGetComponent<Spin>(out Spin spin)) { return; }
        {
            spin.enabled = false;
        }
    }
    private void EnablePlayerControls()
    {
        if (!this.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement)) { return; }
        {
            playerMovement.enabled = true;
        }
        if (!this.TryGetComponent<ThirdPersonControls>(out ThirdPersonControls thirdPersonControls)) { return; }
        {
            thirdPersonControls.enabled = true;
        }
        if (!this.TryGetComponent<PlayerAiming>(out PlayerAiming playerAiming)) { return; }
        {
            playerAiming.enabled = true;
        }
        if (!this.TryGetComponent<ProjectileLauncher>(out ProjectileLauncher projectileLauncher)) { return; }
        {
            projectileLauncher.enabled = true;
        }
        if (!this.TryGetComponent<Spin>(out Spin spin)) { return; }
        {
            spin.enabled = true;
        }
    }
    #endregion
}

[System.Serializable]
public class GoingBack: UnityEvent<GameObject, GameObject>
{
    public GameObject panelToOpen;
    public GameObject panelToClose;
}
