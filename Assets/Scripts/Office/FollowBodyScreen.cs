using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Voice.Unity;

namespace ChiliGames.VROffice
{
    public class FollowBodyScreen : MonoBehaviour
    {
        public Transform[] body;

        PhotonView pv;

        public Text nameText;

        public Text micIcon;
        Recorder recorder;
        public void Setnickname(string nick)
        {
            Debug.Log("name ok : " + nick);
            nameText.text = nick;
        }
        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            recorder = GetComponent<Recorder>();
        }
        public void Muteplayer(bool muteon)
        {
            if (!pv.IsMine) return;
            recorder.TransmitEnabled = !muteon;
        }
        void Update()
        {

        }
    }
}

