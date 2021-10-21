using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.LobbySystemPhoton
{
	public class InitializeRoomStats : MonoBehaviour
	{

		public Text TitleRoom;
		public Text Slogan;
		public PlayerInRoomIcons IconsPlayer;
		private string TitleRoom2;


		// Use this for initialization
		public void Init(string NameRoom, int CountPlayer, int MaxPlayer)
		{
			TitleRoom.text = NameRoom;
			TitleRoom2 = NameRoom;
			Slogan.text = "Player(s) in Room - " + CountPlayer.ToString("00") + "/" + MaxPlayer.ToString("00");
			IconsPlayer.InitIcon(CountPlayer);
		}

		public void ClickedJoinRoom()
		{
			Connexion.instance.theJoinRoom("" + TitleRoom2);
		}
	}
}