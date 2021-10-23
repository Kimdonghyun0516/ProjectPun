using UnityEngine;
using Photon.Pun;

namespace ChiliGames.VROffice
{
    public class Marker : MonoBehaviour
    {
        private Whiteboard whiteboard;
        public Transform drawingPoint;
        public Renderer markerTip;
        private RaycastHit touch;
        bool touching;
        float drawingDistance = 0.015f;
        Quaternion lastAngle;
        PhotonView pv;
        [SerializeField] int penSize = 6;
        [SerializeField] Color color = Color.blue;
        bool grabbed;

        public void ToggleGrab(bool b)
        {
            if (b) grabbed = true;
            else grabbed = false;
        }

        private void Start()
        {
            pv = GetComponent<PhotonView>();
            var block = new MaterialPropertyBlock();

            block.SetColor("_BaseColor", color);

            markerTip.SetPropertyBlock(block);
        }

        void Update()
        {
            if (!pv.IsMine) return;
            if (!grabbed) return;

            if (Physics.Raycast(drawingPoint.position, drawingPoint.up, out touch, drawingDistance))
            {
                if (touch.collider.CompareTag("Finish"))
                {
                    if (!touching)
                    {
                        touching = true;
                        lastAngle = transform.rotation;
                        whiteboard = touch.collider.GetComponent<Whiteboard>();
                    }
                    if (whiteboard == null) return;

                    whiteboard.pv.RPC("DrawAtPosition", RpcTarget.AllBuffered, 
                        new float[] { touch.textureCoord.x, touch.textureCoord.y }, 
                        penSize, 
                        new float[]{color.r, color.g, color.b});
                }
            }
            else if (whiteboard != null)
            {
                touching = false;
                whiteboard.pv.RPC("ResetTouch", RpcTarget.AllBuffered);
                whiteboard = null;
            }

            if (Physics.Raycast(drawingPoint.position, drawingPoint.up, out touch, drawingDistance))
            {
                if (touch.collider.CompareTag("Menu"))
                {
                    if (!touching)
                    {
                        touching = true;
                        lastAngle = transform.rotation;
                        whiteboard = touch.collider.GetComponent<Whiteboard>();
                    }
                    if (whiteboard == null) return;
                    
                }
            }

        }

        private void LateUpdate()
        {
            if (!pv.IsMine) return;

            if (touching)
            {
                transform.rotation = lastAngle;
            }
        }
    }
}

