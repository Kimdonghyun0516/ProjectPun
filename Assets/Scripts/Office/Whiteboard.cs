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

        //RPC sent by the Marker class so every user gets the information to draw in whiteboard.
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

            //If last frame was not touching a marker, we don't need to lerp from last pixel coordinate to new, so we set the last coordinates to the new.
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

        //Reset the state of the whiteboard, so it doesn't interpolate/lerp last pixels drawn.
        [PunRPC]
        public void ResetTouch()
        {
            touchingLastFrame = false;
        }

        //Receives the color from the marker
        public Color[] SetColor(Color color)
        {
            return Enumerable.Repeat(color, penSize * penSize).ToArray();
        }

        //To clear the whiteboard.
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
