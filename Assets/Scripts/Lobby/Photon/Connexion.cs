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

		public static bool Fromoffice = false;
		public static string nicknamecache;

		public override void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		public override void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			nbrPlayersInLobby = PhotonNetwork.CountOfPlayers;

			Debug.Log("isConnected : " + PhotonNetwork.IsConnected + "Server : " + PhotonNetwork.Server);
			if(Fromoffice)
            {
				Template.instance.PlayerNameInput.text = nicknamecache;

				this.OnLoginButtonClicked();
				Debug.Log("Disconnect call");
			}
		}
		public void OnDisconnected()
        {
			Debug.Log("Disconnected");
		}

		public void OnLoginButtonClicked()
		{			
			Template.instance.InputPanel.SetActive(true);
			string playerName = Template.instance.PlayerNameInput.text;

			if (!playerName.Equals(""))
			{
				PhotonNetwork.LocalPlayer.NickName = playerName;
				nicknamecache = playerName;
				PhotonNetwork.OfflineMode = false; 
				PhotonNetwork.AutomaticallySyncScene = true; 
				PhotonNetwork.GameVersion = "v1";

				PhotonNetwork.ConnectUsingSettings();

				Template.instance.InputPanel.SetActive(false);
				Template.instance.Keyboard.SetActive(false);
				Template.instance.LoadingPanel.SetActive(true);
			}
			else
			{
				Template.instance.InputPanel.SetActive(false);
				Template.instance.ErrorPanel.SetActive(true);
				StartCoroutine(Onclick());
				Debug.LogError("닉네임을 입력하세요.");
			}
		}

		IEnumerator Onclick()
        {
			yield return new WaitForSeconds(1.5f);
			Template.instance.ErrorPanel.SetActive(false);
			Template.instance.InputPanel.SetActive(true);
		}

		public override void OnConnectedToMaster()
		{
			Template.instance.LoginPanel.SetActive(false);
			Template.instance.LoadingPanel.SetActive(false);
			Template.instance.Keyboard.SetActive(false);
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

			byte maxPlayers;
			byte.TryParse(Maxplayer.ToString(), out maxPlayers);
			maxPlayers = (byte)Mathf.Clamp(Maxplayer, 2, 8);
			RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

			PhotonNetwork.CreateRoom(roomName, options, null);
			Template.instance.NbrPlayers.text = "00";
			Template.instance.BtnCreatRoom.interactable = false;
		}

		public void OnLeaveGameButtonClicked()
		{
			Application.Quit();
			UnityEditor.EditorApplication.isPlaying = false;
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

		public void BtnShowOffLobby()
		{
			PhotonNetwork.LeaveLobby();
		}
	}
}