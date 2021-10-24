using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Photon.Pun.LobbySystemPhoton
{
	public class Template : MonoBehaviour
	{

		public static Template instance;
		[Header("Input Panel")]
		public GameObject InputPanel;
		public InputField PlayerNameInput;

		[Header("Login Panel")]
		public GameObject LoginPanel;
		public GameObject Keyboard;

		[Header("ListRoom Panel")]
		public GameObject ListRoomPanel;
		public GameObject ListRoomEmpty;
		public Text NbrPlayers;
		public Button BtnCreatRoom;

		[Header("Loading Panel")]
		public GameObject LoadingPanel;

		[Header("Room Panel")]
		public GameObject RoomPanel;
		public Text TitleRoom;

		[Header("Chat Panel")]
		public TMP_Text ChatText;

		void Awake()
		{
			instance = this;
		}
	}
}
