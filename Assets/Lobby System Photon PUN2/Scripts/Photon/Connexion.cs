using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace Photon.Pun.LobbySystemPhoton
{
	public class Connexion : MonoBehaviourPunCallbacks
	{
		public static Connexion instance;
		public int Maxplayer = 8;
		private int nbrPlayersInLobby = 0;

		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;
			//Template.instance.PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
		}

		public void OnLoginButtonClicked()
		{

			string playerName = Template.instance.PlayerNameInput.text;

			if (!playerName.Equals(""))
			{
				PhotonNetwork.LocalPlayer.NickName = playerName;

				PhotonNetwork.OfflineMode = false; //true would "fake" an online connection
												   // PhotonNetwork.NickName = "PlayerName"; //we can use a input to change this 
				PhotonNetwork.AutomaticallySyncScene = true; //To call PhotonNetwork.LoadLevel()
				PhotonNetwork.GameVersion = "v1";

				PhotonNetwork.ConnectUsingSettings();

				Template.instance.InputPanel.SetActive(false);
				Template.instance.LoadingPanel.SetActive(true);
			}
			else
			{
				Debug.LogError("Player Name is invalid.");
			}
		}

		public override void OnConnectedToMaster()
		{
			Template.instance.LoginPanel.SetActive(false);
			Template.instance.LoadingPanel.SetActive(false);
			Template.instance.ListRoomPanel.SetActive(true);
			PhotonNetwork.JoinLobby();
			Debug.Log("OnConnectedToMaster");
			nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;
			Template.instance.NbrPlayers.text = nbrPlayersInLobby.ToString("00");
		}

		public override void OnJoinedLobby()
		{
			Template.instance.BtnCreatRoom.interactable = true;
			Debug.Log("OnJoinedLobby");

		}

		public void OnRefreshButtonClicked()
		{
			PhotonNetwork.LeaveLobby();
			nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;
			Template.instance.NbrPlayers.text = nbrPlayersInLobby.ToString("00");
			PhotonNetwork.JoinLobby();
		}

		public void OnCreateRoomButtonClicked()
		{
			string roomName = PhotonNetwork.LocalPlayer.NickName;
			//roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;
			roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

			byte maxPlayers;
			byte.TryParse(Maxplayer.ToString(), out maxPlayers);
			maxPlayers = (byte)Mathf.Clamp(Maxplayer, 2, 8);
			RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

			PhotonNetwork.CreateRoom(roomName, options, null);
			Template.instance.NbrPlayers.text = "00";
			Template.instance.BtnCreatRoom.interactable = false;
			/*if (!PhotonNetwork.IsConnected)
				return;

			RoomOptions options = new RoomOptions();
			options.MaxPlayers = 10;
			PhotonNetwork.JoinOrCreateRoom("VROffice", options, null);*/
		}

		public void OnLeaveGameButtonClicked()
		{
			PhotonNetwork.LeaveRoom();
		}

		public void theJoinRoom(string roomName)
		{
			PhotonNetwork.JoinRoom(roomName);
		}

		public override void OnLeftRoom()
		{
			Debug.Log("OnLeftRoom");

			SceneManager.LoadScene("index");
		}

		public override void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom");

			SceneManager.LoadScene("Office");
		}
	}
}