using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Mirror;

namespace customLobby {
    public class Lobby : MonoBehaviour {

        public NetworkManagerLobby networkManager = null;

        [SerializeField] private GameObject panel = null;

        public static string DisplayName { get; private set;}

        public TMP_Text nameInputField;

        public TMP_Text displayField;

        public TMP_Text ipAddressField;

        public TMP_Text joinedText;

        public TMP_Text hostingText;

        public TMP_Text lobbyText;

        public TMP_Text numPlayersText;

        public Button continueButton;

        public Button joinButton;

        private const string PlayerPrefKey = "Username";

        private void Start() => SetUp();

        private void SetUp(){
            if (!PlayerPrefs.HasKey(PlayerPrefKey)) {return;}

            string defaultName = PlayerPrefs.GetString(PlayerPrefKey);

            nameInputField.text = defaultName;

            SetPlayerName(defaultName);

            networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManagerLobby>();

        }
        
        private void SetPlayerName(string name){

            if(string.IsNullOrEmpty(name)) { continueButton.gameObject.SetActive(false);}
        }

        public void SavePlayerName(){
            name = displayField.text;
            DisplayName = name;
            PlayerPrefs.SetString(PlayerPrefKey,name);
        }

        public void HostLobby() {
            networkManager.StartHost();
            setHostingText();
            Debug.Log(networkManager.isNetworkActive + " " + networkManager.networkAddress);
        }

        public void OnEnable() {
            NetworkManagerLobby.OnClientConnected += HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
        }

        public void OnDisable() {
            NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
            NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby() {
            string ipAddress = ipAddressField.text;

            ipAddress.Trim();

            networkManager.StartClient();
            networkManager.networkAddress = ipAddress;

            //Debug.Log(networkManager.isNetworkActive + " " + networkManager.networkAddress + " " + networkManager.numPlayers);
        }

        public void leaveLobby() {
            networkManager.StopClient();
            Debug.Log("Leaving lobby");
        }

        private void HandleClientConnected() {
            Debug.Log("Client connected!");
            lobbyText.gameObject.SetActive(false);
            panel.SetActive(false);
            updateNumPlayers();

        }

        private void HandleClientDisconnected() {
            Debug.Log("Client disconnected!");
            joinedText.gameObject.SetActive(false);
            lobbyText.gameObject.SetActive(true);
            panel.SetActive(true);
            updateNumPlayers();
            numPlayersText.gameObject.SetActive(false);
        }

        public void setJoinedText() {
            joinedText.gameObject.SetActive(true);
            joinedText.text = "Joined: " + networkManager.networkAddress;
            updateNumPlayers();
        }

        private void setHostingText() {
            hostingText.gameObject.SetActive(true);
            hostingText.text = "Hosting on: " + networkManager.networkAddress;
            updateNumPlayers();
        }

        private void updateNumPlayers() {
            numPlayersText.SetText("Number of Players: " + networkManager.numPlayers);
        }

    }
}
