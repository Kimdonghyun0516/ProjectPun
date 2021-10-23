using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ChiliGames.VROffice
{
    public class RayController : MonoBehaviour
    {
        public XRController leftTeleportRay;
        public XRController rightTeleportRay;
        public InputHelpers.Button teleportActivationButton;
        public float activationThreshold = 0.1f;
        public bool leftTeleportEnabled { get; set; } = true;
        public bool rightTeleportEnabled { get; set; } = true;
        private bool leftButtonPressedLastFrame = false;
        private bool rightButtonPressedLastFrame = false;
        public GameObject leftTeleportReticle;
        public GameObject rightTeleportReticle;

        void Start()
        {
            InitializeTeleportRay(leftTeleportRay);
            InitializeTeleportRay(rightTeleportRay);
        }

        void InitializeTeleportRay(XRController teleportRay)
        {
            if (!teleportRay) { return; }

            teleportRay.gameObject.SetActive(false);
        }

        void Update()
        {
            ManageTeleportRay(leftTeleportRay, ref leftButtonPressedLastFrame, leftTeleportReticle, leftTeleportEnabled);
            ManageTeleportRay(rightTeleportRay, ref rightButtonPressedLastFrame, rightTeleportReticle, rightTeleportEnabled);
        }

        void ManageTeleportRay(XRController teleportRay, ref bool buttonPressedLastFrame, GameObject teleportReticle, bool teleportEnabled)
        {
            if (!teleportRay) { return; }

            InputHelpers.IsPressed(teleportRay.inputDevice, teleportActivationButton, out bool isPressed, activationThreshold);

            bool buttonJustPressed = isPressed && !buttonPressedLastFrame;
            bool buttonJustReleased = !isPressed && buttonPressedLastFrame;

            if (buttonJustPressed && teleportEnabled)
            {
                teleportRay.gameObject.SetActive(true);
                teleportReticle.SetActive(false);
            }
            else if (buttonJustReleased)
            {
                SetActiveNextFrame(teleportRay.gameObject, false);
            }

            buttonPressedLastFrame = isPressed;
        }

        public void SetActiveNextFrame(GameObject gameObject, bool active)
        {
            StartCoroutine(SetActiveNextFrameHelper(gameObject, active));
        }

        IEnumerator SetActiveNextFrameHelper(GameObject gameObject, bool active)
        {
            yield return null;
            gameObject.SetActive(active);
        }
    }
}