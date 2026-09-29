using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMenu : NetworkBehaviour
{
    //IN THIS SOLUTION THE ASSUMPTION THAT THE MAIN MENU PANEL IS ALWAYS THE FIRST INDEX IS IMPORTANT FOR THE REST OF THE LOGIC TO RUN
    [SerializeField] InputReader inputReader;
    [SerializeField] GameObject playerReference;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject HowToPlayMenu;
    [SerializeField] GameObject SettingsMenu;

    private bool MenuState;
    private bool isMainMenuOpen;
    [SerializeField] private string ControlPanelName;


    [Header("Device Layouts")]
    // intent is to have only 2 panels for each layout
    [SerializeField] GameObject gamePadPanelContainer;
    [SerializeField] GameObject keyboardPanelContainer;
    [SerializeField] GameObject[] gamePadPanels;
    [SerializeField] GameObject[] keyboardPanels;
    [SerializeField] TextMeshProUGUI FlightTypeText;
    private bool isGamepadLayout;

    [Header("Menu Audio")]
    [SerializeField] AudioSource MenuAudio;
    [SerializeField] AudioClip MenuOpenClose;
    [SerializeField] AudioClip ButtonClick;
    [SerializeField] AudioClip BackButtonClick;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) {  return; }
        MainMenu.SetActive(false);
        inputReader.OnMenuEvent += TriggerMainMenu;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.OnMenuEvent -= TriggerMainMenu;
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
    public void TriggerMainMenu()
    {
        if (!IsOwner) { return; }
        MenuAudio.PlayOneShot(MenuOpenClose);
        MenuState = !MenuState;

        if (MenuState == true)
        {
            isMainMenuOpen = true;
            DisablePlayerControls();
            MainMenu.SetActive(true);
            mainMenuCanvas.transform.GetChild(0).gameObject.SetActive(true);
            //if (isMainMenuOpen == true)
            //{
            //    for (int i = 1; i < mainMenuCanvas.transform.childCount; i++)
            //    {
            //        mainMenuCanvas.transform.GetChild(0).gameObject.SetActive(false);
            //    }
            //}

        }
        if (MenuState == false)
        {
            EnablePlayerControls();
            isMainMenuOpen = false;
            if (isMainMenuOpen == false)
            {
                
                MainMenu.SetActive(false);
                for (int i = 0; i < mainMenuCanvas.transform.childCount; i++)
                {
                    mainMenuCanvas.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        

       
    }

    #region UI Navigation
    public void ResumeGame()
    {
        if (!IsOwner) { return; }
        MenuAudio.PlayOneShot(MenuOpenClose);
        MainMenu.SetActive(false);
        MenuState = false;
        EnablePlayerControls();
    }

    public void OpenPanel(int panelIndex)
    {
        if (!IsOwner) { return; }
        for (int i = 0;  i < mainMenuCanvas.transform.childCount; i++)
        {
            if (i == panelIndex)
            {
                MenuAudio.PlayOneShot(ButtonClick);
                mainMenuCanvas.transform.GetChild(i).gameObject.SetActive(true);

                if(mainMenuCanvas.transform.GetChild(i).gameObject.name == ControlPanelName)
                {
                    Debug.Log("Name of object is " + mainMenuCanvas.transform.GetChild(i).gameObject.name);
                    ShowGamepadLayout();
                }
            }
            else
            {
                mainMenuCanvas.transform.GetChild(i).gameObject.SetActive(false) ;
            }
        }
    }

    public void BackButton(GameObject currentPanelToClose)
    {
        if (!IsOwner) { return; }
        MenuAudio.PlayOneShot(BackButtonClick);
        currentPanelToClose.SetActive(false);
        MainMenu.SetActive(true);
    }
     
    #endregion


    #region PlayerControls
    private void DisablePlayerControls()
    {

        playerReference.GetComponent<PlayerMovement>().enabled = false;
        playerReference.GetComponent<ThirdPersonControls>().enabled = false;
        playerReference.GetComponent<PlayerAiming>().enabled = false;
        playerReference.GetComponent<ProjectileLauncher>().enabled = false;
        playerReference.GetComponent<Spin>().enabled = false;

    }
    private void EnablePlayerControls()
    {
        playerReference.GetComponent<PlayerMovement>().enabled = true;
        playerReference.GetComponent<ThirdPersonControls>().enabled = true;
        playerReference.GetComponent<PlayerAiming>().enabled = true;
        playerReference.GetComponent<ProjectileLauncher>().enabled = true;
        playerReference.GetComponent<Spin>().enabled = true;
    }
    #endregion


    #region Layouts

    public void ShowGamepadLayout()
    {
        if (!IsOwner) { return; }
        MenuAudio.PlayOneShot(ButtonClick);
        isGamepadLayout = true;
        gamePadPanelContainer.SetActive(isGamepadLayout);
        keyboardPanelContainer.SetActive(!isGamepadLayout);
        ShowGamePadDefaultLayout();
    }
    public void ShowKeyboardLayout()
    {
        if (!IsOwner) { return; }
        MenuAudio.PlayOneShot(ButtonClick);
        isGamepadLayout = false;
        keyboardPanelContainer.SetActive(!isGamepadLayout);
        gamePadPanelContainer.SetActive(isGamepadLayout);
        ShowKeyboardDefaultLayout();
    }
    private void ShowGamePadDefaultLayout()
    {
        FlightTypeText.text = "Top Down";
        gamePadPanels[0].SetActive(true);
        gamePadPanels[1].SetActive(false);
    }
    private void ShowKeyboardDefaultLayout()
    {
        FlightTypeText.text = "Top Down";
        keyboardPanels[0].SetActive(true);
        keyboardPanels[1].SetActive(false);
    }

    public void PreviousLayout()
    {
        if (!IsOwner) { return; }
        FlightTypeText.text = "Top Down";
        MenuAudio.PlayOneShot(ButtonClick);
        if (isGamepadLayout == true)
        {
            gamePadPanels[0].SetActive(true);
            gamePadPanels[1].SetActive(false);
        }
        else
        {
            keyboardPanels[0].SetActive(true);
            keyboardPanels[1].SetActive(false);
        }
    }
    public void NextLayout()
    {
        if (!IsOwner) { return; }
        FlightTypeText.text = "Free Flight";
        MenuAudio.PlayOneShot(ButtonClick);
        if (isGamepadLayout == true)
        {
            gamePadPanels[0].SetActive(false);
            gamePadPanels[1].SetActive(true);
        }
        else
        {
            keyboardPanels[0].SetActive(false);
            keyboardPanels[1].SetActive(true);
        }
    }
    #endregion
}

