using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class RigButtondown : MonoBehaviour
{
    InputFeatureUsage<bool>[] button_arr;
    List<InputDevice> devices;
    public InputDeviceRole deviceRole;
    public UnityEvent Primary;

    public XRController controller = null;

    private int Num = 2;
    private int time = 0;

    private bool input_Key = false;
    private void Awake()
    {
        button_arr = new InputFeatureUsage<bool>[Num];
        button_arr[0] = CommonUsages.primaryButton;
        button_arr[1] = CommonUsages.secondaryButton;
        devices = new List<InputDevice>();
    }
    void Start()
    {
        
    }

    private void Update()
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
                for (int j = 0; j < Num; j++)
                {
                    devices[i].TryGetFeatureValue(button_arr[j], out input_Key);
                    if (input_Key && j == 0)
                    {
                        time = 60;
                        Primary.Invoke();
                        Debug.Log("Primary_Button");
                    }
                    else if (input_Key && j == 1)
                    {
                        time = 60;
                        Primary.Invoke();
                        Debug.Log("secondary_Button");
                    }
                }
            }
        }
    }
    private void CommonInput()
    {
        
        // A Button
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primary)) 
        {
            Debug.Log("aaaaaaaaaaa");
        }
        //output += "A Pressed: " + primary + "\n";

        // B Button
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondary)) { }
        // output += "B Pressed: " + secondary + "\n";

        // Touchpad/Joystick touch
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool touch)) { }
        // output += "Touchpad/Joystick Touch: " + touch + "\n";

        // Touchpad/Joystick press
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool press))
        { }
        // output += "Touchpad/Joystick Pressed: " + press + "\n";

        // Touchpad/Joystick position
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
        { }
        // output += "Touchpad/Joystick Position: " + position + "\n";

        // Grip press
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool grip)) { }
        //  output += "Grip Pressed: " + grip + "\n";

        // Grip amount
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripAmount)) { }
        //  output += "Grip Amount: " + gripAmount + "\n";

        // Trigger press
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger)) { }
        // output += "Trigger Pressed: " + trigger + "\n";

        // Index/Trigger amount
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerAmount)) { }
        //  output += "Trigger: " + triggerAmount;
    }
}

