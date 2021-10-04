using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.LobbySystemPhoton
{
	public class ListRoom : MonoBehaviourPunCallbacks
	{

		public static ListRoom instance;

		[Header("Room and CachedRoom")]
		public List<CachedRoom> cacheRoomList;
		public List<RoomEntry> roomEntryList;

		[Header("Room List Panel")]
		public GameObject RoomListContent;
		public GameObject RoomListEntryPrefab;

		CachedRoom TmpCache;

		void Awake()
		{
			instance = this;
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			ClearRoomListView();
			UpdateCachedRoomList(roomList);
			UpdateRoomListView();
		}

		public void ClearRoomListView()
		{
			foreach (RoomEntry entry in roomEntryList)
			{
				Destroy(entry.obj);
			}
			roomEntryList.Clear();
			cacheRoomList.Clear();
		}

		private void UpdateCachedRoomList(List<RoomInfo> roomList)
		{
			foreach (RoomInfo info in roomList)
			{

				// Remove room from cached room list if it got closed, became invisible or was marked as removed
				if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
				{
					if (cacheRoomList.Contains(TmpCache))
					{
						cacheRoomList.Remove(TmpCache);
					}
					continue;
				}

				foreach (CachedRoom im in cacheRoomList)
				{
					if (info.Name == im.name)
					{
						TmpCache = im;
					}
				}

				// Update cached room info
				if (cacheRoomList.Contains(TmpCache))
				{
					TmpCache.rInfo = info;
				}
				// Add new room info to cache
				else
				{
					CachedRoom cacheRoom = new CachedRoom();
					cacheRoom.name = info.Name;
					cacheRoom.countplayer = info.PlayerCount;
					cacheRoom.maxplayer = info.MaxPlayers;
					cacheRoom.rInfo = info;
					cacheRoomList.Add(cacheRoom);
				}
			}
		}

		private void UpdateRoomListView()
		{
			foreach (CachedRoom info in cacheRoomList)
			{
				GameObject entry = Instantiate(RoomListEntryPrefab) as GameObject;
				entry.transform.SetParent(RoomListContent.transform);
				entry.transform.localScale = Vector3.one;
				entry.GetComponent<InitializeRoomStats>().Init(info.name, info.countplayer, info.maxplayer);

				RoomEntry roomEntry = new RoomEntry();
				roomEntry.name = info.name;
				roomEntry.obj = entry;
				roomEntryList.Add(roomEntry);
			}
			Template.instance.ListRoomEmpty.SetActive(checkedNbrRoom());
		}

		private bool checkedNbrRoom()
		{
			if (cacheRoomList.Count <= 0)
			{
				return true;
			}

			return false;
		}

		[System.Serializable]
		public struct CachedRoom
		{
			public string name;
			public int countplayer;
			public int maxplayer;
			public RoomInfo rInfo;
		}

		[System.Serializable]
		public struct RoomEntry
		{
			public string name;
			public GameObject obj;
		}
	}
}