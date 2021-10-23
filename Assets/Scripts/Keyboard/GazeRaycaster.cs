using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace VRKeyboard.Utils
{
    public class GazeRaycaster : MonoBehaviour
    {
        //public float delayInSeconds;
        //public float loadingTime;
        public Image circle;

        private string lastTargetName = "";
        Coroutine Control; // Keep a single gaze control coroutine for better performance.

        public XRNode inputs;

        //private float wait = 0.9f;
        #region MonoBehaviour Callbacks
        void FixedUpdate()
        {
            RaycastHit hit;

            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            
            InputDevice device = InputDevices.GetDeviceAtXRNode(inputs);
            bool is_trigger = false;
            device.TryGetFeatureValue(CommonUsages.triggerButton, out is_trigger);

            if (Physics.Raycast(transform.position, fwd, out hit))
            {
                if (is_trigger)//&& Time.time > wait)
                {
                    // 키 또는 작동 버튼을 누른 경우에만 이벤트를 트리거합니다.
                    if (hit.transform.tag == "VRGazeInteractable")
                    {
                        // Check if we have already gazed over the object.
                        if (lastTargetName == hit.transform.name)
                        {
                            return;
                        }

                        // Set the last hit if last targer is empty
                        if (lastTargetName == "")
                        {
                            lastTargetName = hit.transform.name;
                        }

                        // Check if current hit is same with last one;
                        if (hit.transform.name != lastTargetName)
                        {
                            circle.fillAmount = 0f;
                            lastTargetName = hit.transform.name;
                        }

                        if (null != Control)
                        {
                            StopCoroutine(Control);
                        }
                        Control = StartCoroutine(FillCircle(hit.transform));

                        return;
                    }
                    else
                    {
                        if (null != Control)
                        {
                            StopCoroutine(Control);
                        }
                        ResetGazer();
                    }
                }
            }
            else
            {
                if (null != Control)
                {
                    StopCoroutine(Control);
                }
                ResetGazer();
            }
        }
        #endregion

        #region Private Methods
        private IEnumerator FillCircle(Transform target)
        {
            // When the circle starts to fill, reset the timer.
            
            //circle.fillAmount = 0f;

            //yield return new WaitForSeconds(delayInSeconds);
          
                yield return null;          

            //circle.fillAmount = 1f;

            if (target.GetComponent<Button>())
            {
                target.GetComponent<Button>().onClick.Invoke();
                yield return new WaitForSeconds(1.8f);
            }
            ResetGazer();
        }

        // Reset the loading circle to initial, and clear last detected target.
        private void ResetGazer()
        {
            if (circle == null)
            {
                Debug.LogError("Please assign target loading image, (ie. circle image)");
                return;
            }

            circle.fillAmount = 0f;
            lastTargetName = "";
        }
        #endregion
    }
}