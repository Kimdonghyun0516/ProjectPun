using UnityEngine;

namespace Photon.Pun.LobbySystemPhoton
{
	public class TimerNbrPlayers : MonoBehaviourPunCallbacks
	{
		public float time;

		// 업데이트는 프레임당 한 번씩 호출
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