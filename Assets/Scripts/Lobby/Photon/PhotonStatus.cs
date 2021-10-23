using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.LobbySystemPhoton
{
	public class PhotonStatus : MonoBehaviour
	{
		private readonly string connectionStatusMessage = "    Connection Status: ";

		[Header("UI References")]
		public Text ConnectionStatusText;

		public void Update()
		{
            if (PhotonNetwork.IsConnected)
            {
                ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
            }
		}

	}
}