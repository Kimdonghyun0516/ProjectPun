using UnityEngine;

namespace Photon.Pun.LobbySystemPhoton
{
	public class TimerNbrPlayers : MonoBehaviourPunCallbacks
	{
		public float time;

		// Update is called once per frame
		void Update()
		{
			if (PhotonNetwork.InLobby)
			{
				time -= Time.deltaTime;
				if (time <= 0f)
				{
					time = 5f;
					Template.instance.NbrPlayers.text = PhotonNetwork.CountOfPlayers.ToString("00");
				}
			}
		}
	}
}