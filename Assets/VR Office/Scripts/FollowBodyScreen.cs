using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Voice.Unity;

namespace ChiliGames.VROffice
{
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
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
        // Update is called once per frame
        void Update()
        {
            if (!pv.IsMine) return;
            for (int i = 0; i < body.Length; i++)
            {
                body[i].position = PlatformManager.instance.screenRigParts[i].position;
                body[i].rotation = PlatformManager.instance.screenRigParts[i].rotation;
            }

            float amp = recorder.LevelMeter.CurrentAvgAmp;
            if(amp >= 0.001f)
            {
                micIcon.text = "말 O";
            }
            else
            {
                micIcon.text = "말 X";
            }
        }
    }
}

