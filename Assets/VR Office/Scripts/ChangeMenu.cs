using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using System;

public class ChangeMenu : MonoBehaviour
{
    InputFeatureUsage<bool> button;
    List<InputDevice> devices;
    public InputDeviceRole deviceRole;
    public GameObject Primary;

    private int Num = 2;
    private int time = 0;
    private bool input_Key = false;
    private bool setObject = true;
    void Awake()
    {
        button = new InputFeatureUsage<bool>();
        button = CommonUsages.primaryButton;
        devices = new List<InputDevice>();

    }
    void Update()
    {
        if (time >= 0)
        {
            time--;
            return;
        }
        if (time <= 0)
        {
            InputDevices.GetDevicesWithRole(deviceRole, devices);
            for (int i = 0; i < devices.Count; i++)
            {
                devices[i].TryGetFeatureValue(button, out input_Key);
                if (input_Key)
                {
                    if (setObject)
                        setObject = false;
                    else
                        setObject = true;
                    time = 60;
                    Primary.SetActive(setObject);
                    Debug.Log("Primary_Button");
                }
            }
        }
    }
}