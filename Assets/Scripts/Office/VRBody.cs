﻿using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Voice.Unity;
using System;

namespace ChiliGames.VROffice
{
    public class VRBody : MonoBehaviourPunCallbacks
    {
        public Transform[] body;
        [SerializeField] SkinnedMeshRenderer lHand;
        [SerializeField] SkinnedMeshRenderer rHand;
        [SerializeField] SkinnedMeshRenderer bodyRenderer;

        private Color playerColor;

        PhotonView pv;

        public Text nameText;

        public Text micIcon;
        Recorder recorder;

        string nickText;

        [PunRPC]
        public void RPC_NickName(string nick)
        {
            nickText = nick;
        }
        public void Setnickname(string nick)
        {
            pv.RPC("RPC_NickName", RpcTarget.AllBuffered, nick);
            nameText.text = nick;
            Debug.Log("Setnickname" + nick);
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

            if (pv.IsMine)
            {
                lHand.enabled = true;
                rHand.enabled = true;
            }

            if (PlatformManager.instance != null)
            {
                PlatformManager.instance.onSpawned.AddListener(SetColor);
            }
        }

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
        }
        long speakingtime = 0;
        bool lastspeakingstatus = false;
        private void FixedUpdate()
        {
            micIcon.text = micText;
            nameText.text = nickText;

            float amp = recorder.LevelMeter.CurrentAvgAmp;

            bool s = amp >= 0.005f && recorder.TransmitEnabled;
            if (s)
            {
                speakingtime = DateTime.Now.Ticks + 1000000 * (long)(10 * 1); // 1초
            }
            bool mutespeakingtime = DateTime.Now.Ticks < speakingtime;

            if (mutespeakingtime != lastspeakingstatus) // 필터링
            {
                pv.RPC("RPC_Speaking", RpcTarget.AllBuffered, mutespeakingtime);
                lastspeakingstatus = mutespeakingtime;
            }
        }

        string micText = "/";

        [PunRPC]
        public void RPC_Speaking(bool time)
        {
            Debug.Log("RPC_Speaking" + time);
            if (time)
            {
                micText = "";
            }
            else
            {
                micText = "/";
            }
        }

        [PunRPC]
        public void RPC_TeleportEffect()
        {
            StopAllCoroutines();
            StartCoroutine(TeleportEffect());
        }

        IEnumerator TeleportEffect()
        {
            float effectDuration = 0.8f;
            for (float i = 0; i < effectDuration; i += Time.deltaTime)
            {
                bodyRenderer.material.SetFloat("_CutoffHeight", Mathf.Lerp(-1, 4, i / effectDuration));
                yield return null;
            }
        }

        //룸에 참여하는 각 플레이어의 색을 다르게 설정
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

            //몸과 손의 색을 설정
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