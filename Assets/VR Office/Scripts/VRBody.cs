using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Voice.Unity;

namespace ChiliGames.VROffice
{
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
    public class VRBody : MonoBehaviourPunCallbacks
    {
        public Transform[] body;
        [SerializeField] SkinnedMeshRenderer lHand;
        [SerializeField] SkinnedMeshRenderer rHand;
        [SerializeField] SkinnedMeshRenderer bodyRenderer;

        private Color playerColor;

        PhotonView pv;

        public InputField nameText;

        public Text micIcon;
        Recorder recorder;

        public void Setnickname(string nick)
        {
            Debug.Log("name ok : " + nick);
            nameText.text = nick;
        }
        public void Muteplayer(bool muteon)
        {
            if (!pv.IsMine) return;
            recorder.TransmitEnabled = !muteon;

        }

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            recorder = GetComponent<Recorder>();

            //Enable hand renderers if this is my avatar.
            if (pv.IsMine)
            {
                lHand.enabled = true;
                rHand.enabled = true;
            }

            if(PlatformManager.instance != null)
            {
                PlatformManager.instance.onSpawned.AddListener(SetColor);
            }
        }


        void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);       
            Debug.Log("OnEnableVRbody");

        }
        // Follow trackers only if it's our body
        void Update()
        {
            if (pv.IsMine)
            {
                for (int i = 0; i < body.Length; i++)
                {
                    body[i].position = PlatformManager.instance.vrRigParts[i].position;
                    body[i].rotation = PlatformManager.instance.vrRigParts[i].rotation;
                }
            }

            float amp = recorder.LevelMeter.CurrentAvgAmp;
            if (amp >= 0.001f)
            {
                micIcon.text = "말 O";
            }
            else
            {
                micIcon.text = "말 X";
            }
        }

        [PunRPC]
        public void RPC_TeleportEffect()
        {
            StopAllCoroutines();
            StartCoroutine(TeleportEffect());
        }

        //Lerps the dissolve shader to create a teleportation effect on the avatar.
        IEnumerator TeleportEffect()
        {
            float effectDuration = 0.8f;
            for (float i = 0; i < effectDuration; i += Time.deltaTime)
            {
                bodyRenderer.material.SetFloat("_CutoffHeight", Mathf.Lerp(-1, 4, i / effectDuration));
                yield return null;
            }
        }

        //For setting different colors to each player joining the room.
        void SetColor()
        {
            Debug.Log("Setting color " + PlatformManager.instance.spawnPosIndex);
            pv.RPC("RPC_SetColor", RpcTarget.AllBuffered, PlatformManager.instance.spawnPosIndex);
        }

        [PunRPC]
        void RPC_SetColor(int n)
        {
            n++;
            switch (n)
            {
                case 1:
                    playerColor = Color.red;
                    break;
                case 2:
                    playerColor = Color.cyan;
                    break;
                case 3:
                    playerColor = Color.green;
                    break;
                case 4:
                    playerColor = Color.yellow;
                    break;
                case 5:
                    playerColor = Color.magenta;
                    break;
                case 6:
                    playerColor = Color.blue;
                    break;
                case 7:
                    playerColor = Color.Lerp(Color.yellow, Color.red, 0.5f);
                    break;
                case 8:
                    playerColor = Color.Lerp(Color.blue, Color.red, 0.5f);
                    break;
                case 9:
                    playerColor = Color.Lerp(Color.red, Color.green, 0.5f);
                    break;
                default:
                    playerColor = Color.black;
                    break;
            }
            playerColor = Color.Lerp(Color.white, playerColor, 0.5f);

            //Set body and hands color.
            bodyRenderer.material.SetColor("_Albedo", playerColor);
            lHand.material.SetColor("_BaseColor", playerColor);
            rHand.material.SetColor("_BaseColor", playerColor);
        }


        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoomVRbody");

            SceneManager.LoadScene("index");
        }
    }
}
