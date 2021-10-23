using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace VRKeyboard.Utils
{
    public class GazeRaycaster : MonoBehaviour
    {
        public Image circle;

        private string lastTargetName = "";
        Coroutine Control;

        public XRNode inputs;

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
                if (is_trigger)
                {
                    // 키 또는 작동 버튼을 누른 경우에만 이벤트를 트리거합니다.
                    if (hit.transform.tag == "VRGazeInteractable")
                    {

                        if (lastTargetName == hit.transform.name)
                        {
                            return;
                        }


                        if (lastTargetName == "")
                        {
                            lastTargetName = hit.transform.name;
                        }


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
                yield return null;          

            if (target.GetComponent<Button>())
            {
                target.GetComponent<Button>().onClick.Invoke();
                yield return new WaitForSeconds(1.8f);
            }
            ResetGazer();
        }

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