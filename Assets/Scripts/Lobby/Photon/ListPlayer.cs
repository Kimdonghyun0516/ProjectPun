using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Photon.Pun.LobbySystemPhoton
{
	public class ListPlayer : MonoBehaviourPunCallbacks
	{
		[Header("Inside Room Panel")]
		public GameObject InsideRoomPanel;

		public GameObject PlayerListEntryPrefab;
		public Dictionary<int, GameObject> playerListEntries;
		public TChat chat;

		public override void OnJoinedRoom()
		{
			return;
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			GameObject entry = Instantiate(PlayerListEntryPrefab);
			entry.transform.SetParent(InsideRoomPanel.transform);
			entry.transform.localScale = Vector3.one;
			if (newPlayer.IsMasterClient)
			{
				entry.GetComponent<TMP_Text>().text = "<color=#a52a2aff>" + newPlayer.NickName + "</color>";
			}
			else
			{
				entry.GetComponent<TMP_Text>().text = newPlayer.NickName;
			}

			playerListEntries.Add(newPlayer.ActorNumber, entry);
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
			playerListEntries.Remove(otherPlayer.ActorNumber);
		}

		public override void OnLeftRoom()
		{

		}
	}
}
