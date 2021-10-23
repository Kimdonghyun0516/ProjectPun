using UnityEngine;

namespace ChiliGames.VROffice
{
    [System.Serializable]
    public class VRMap
    {
        public Transform vrTarget;
        public Transform rigTarget;
        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;

        public void Map()
        {
            if (vrTarget == null) return;
            rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }

    public class Avatar : MonoBehaviour
    {
        public VRMap head;
        public VRMap leftHand;
        public VRMap rightHand;

        public Transform headConstraint;
        Vector3 headBodyOffset;
        private float turnSmoothness = 3f;

        void Start()
        {
            headBodyOffset = transform.position - headConstraint.position;

            if (GetComponentInParent<Photon.Pun.PhotonView>().IsMine)
            {
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
        }

        void Update()
        {
            transform.position = headConstraint.position + headBodyOffset;
            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

            head.Map();
            leftHand.Map();
            rightHand.Map();
        }
    }
}

