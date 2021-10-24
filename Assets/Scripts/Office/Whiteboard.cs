using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace ChiliGames.VROffice
{
    public class Whiteboard : MonoBehaviourPunCallbacks
    {
        private int textureSize = 2048;
        private int penSize = 6;
        private int penSizeD2 = 3;
        private Texture2D texture;
        private Color[] color;
        private Color[] deleteColor;
        new Renderer renderer;

        private bool touchingLastFrame;
        public bool touching;

        private float lastX, lastY;
        bool everyOthrFrame;

        [HideInInspector] public PhotonView pv;
        private Texture2D receivedTexture;

        void Start()
        {
            renderer = GetComponent<Renderer>();
            texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

            texture.filterMode = FilterMode.Trilinear;
            texture.anisoLevel = 3;

            renderer.material.mainTexture = texture;

            Color fillColor = Color.white;
            var fillColorArray = texture.GetPixels();

            for (var i = 0; i < fillColorArray.Length; ++i)
            {
                fillColorArray[i] = fillColor;
            }

            texture.SetPixels(fillColorArray);

            texture.Apply();

            pv = GetComponent<PhotonView>();
            Debug.Log("PhotonView = " +pv);


            deleteColor = Enumerable.Repeat(Color.white, textureSize * textureSize).ToArray();
        }

        public void SetPenSize(int n)
        {
            penSize = n;
            penSizeD2 = n / 2;
        }

        //모든 사용자가 화이트보드에 그릴 정보를 얻도록 Marker 클래스에서 RPC를 발송
        [PunRPC]
        public void DrawAtPosition(float[] pos, int _pensize, float[] _color)
        {
            SetPenSize(_pensize);
            color = SetColor(new Color(_color[0], _color[1], _color[2]));

            int x = (int)(pos[0] * textureSize - penSizeD2);
            int y = (int)(pos[1] * textureSize - penSizeD2);

            x = Mathf.Max(x, 0);
            x = Mathf.Min(x, 2000);
            y = Mathf.Max(y, 0);
            y = Mathf.Min(y, 2000);

            //마지막 프레임이 마커에 닿지 않았다면
            //마지막 픽셀 좌표에서 새 픽셀로 이동할 필요가 없으므로
            //마지막 좌표를 새 좌표로 설정
            if (!touchingLastFrame)
            {
                lastX = (float)x;
                lastY = (float)y;
                touchingLastFrame = true;
            }

            if (touchingLastFrame)
            {
                /*string str1 = string.Format("setFixel {0}, {1}, {2}, {3}", x, y, penSize, color[0]);
                Debug.Log(str1);*/

                texture.SetPixels(x, y, penSize, penSize, color);

                for (float t = 0.01f; t < 1.00f; t += 0.1f)
                {
                    int lerpX = (int)Mathf.Lerp(lastX, (float)x, t);
                    int lerpY = (int)Mathf.Lerp(lastY, (float)y, t);

                    lerpX = Mathf.Max(lerpX, 0);
                    lerpX = Mathf.Min(lerpX, 2000);
                    lerpY = Mathf.Max(lerpY, 0);
                    lerpY = Mathf.Min(lerpY, 2000);

                    texture.SetPixels(lerpX, lerpY, penSize, penSize, color);
                }
                if (!everyOthrFrame)
                {
                    everyOthrFrame = true;
                }
                else if (everyOthrFrame)
                {
                    texture.Apply();
                    everyOthrFrame = false;
                }
            }

            lastX = (float)x;
            lastY = (float)y;
        }

        //화이트보드의 상태를 재설정하여 그려진 마지막 픽셀을  interpolate/lerp 하지 않도록 한다.
        [PunRPC]
        public void ResetTouch()
        {
            touchingLastFrame = false;
        }

        //마커에서 색상을 수신
        public Color[] SetColor(Color color)
        {
            return Enumerable.Repeat(color, penSize * penSize).ToArray();
        }

        //화이트보드 지우기
        public void ClearWhiteboard()
        {
            //Debug.Log("ClearWhiteboard = " + pv);

            pv.RPC("RPC_ClearWhiteboard", RpcTarget.AllBuffered);

        }

        bool IsLeftMouseButtonDown()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.leftButton.isPressed : false;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        [PunRPC]
        public void RPC_ClearWhiteboard()
        {
            texture.SetPixels(deleteColor);
            texture.Apply();
        }

        
        public void WhiteBoardstart(bool on)
        {
            Debug.Log("WhiteBoardstart"+ on);

            Whiting = on;
        }

        bool Whiting = false;


        private void FixedUpdate()
        {

        }
    }
}
