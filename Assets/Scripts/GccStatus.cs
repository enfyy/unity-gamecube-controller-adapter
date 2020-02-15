using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GccStatus
{
    public int PortIndex;
    public static byte[] ControllerData;
    public bool IsActive;
    
    public float Left_xVal;
    public float Left_yVal;
    public float Right_xVal;
    public float Right_yVal;
    
    public float Left_Trigger_Val;
    public float Right_Trigger_Val;
    
    //Buttons
    public bool Button_A;
    public bool Button_B;
    public bool Button_X;
    public bool Button_Y;
    
    public bool Button_LEFT;
    public bool Button_RIGHT;
    public bool Button_DOWN;
    public bool Button_UP;
    
    public bool Button_START;
    public bool Button_Z;
    public bool Button_R;
    public bool Button_L;

    public static GccStatus[] ProcessControllerData(byte[] controllerData)
    {
        GccStatus[] ports = new GccStatus[4];
        ControllerData = controllerData;
        
        byte[] port1 = new byte [8];
        byte[] port2 = new byte [8];
        byte[] port3 = new byte [8];
        byte[] port4 = new byte [8];
            
        Array.Copy(controllerData, 2, port1, 0, 8);
        Array.Copy(controllerData, 11, port2, 0, 8);
        Array.Copy(controllerData, 20, port3, 0, 8);
        Array.Copy(controllerData, 29, port4, 0, 8);
        
        ports[0] = new GccStatus(port1, 1);
        ports[1] = new GccStatus(port2, 2);
        ports[2] = new GccStatus(port3, 3);
        ports[3] = new GccStatus(port4, 4);
        return ports;
    }

    public GccStatus(byte[] controllerPortData, int index)
    {
        PortIndex = index;
        if (!CheckActive(controllerPortData))
            return;
        
        AssignValuesFromData(controllerPortData);
        Assign_Buttons_A_B_X_Y(controllerPortData);
        Assign_Buttons_DPAD(controllerPortData);
        Assign_Buttons_START_Z_R_L(controllerPortData);
    }

    private bool CheckActive(byte [] controllerPortData)
    {
        foreach (var data in controllerPortData)
        {
            if (data != 0)
            {
                IsActive = true;
                return true;
            }
        }

        IsActive = false;
        return false;
    }

    private void AssignValuesFromData(byte [] controllerPortData)
    {
        //read data and ste vals
        Left_xVal         = (float) ( controllerPortData[2] - 128 )/100;
        Left_yVal         = (float) ( controllerPortData[3] - 128 )/100;
        Right_xVal        = (float) ( controllerPortData[4] - 128 )/100;
        Right_yVal        = (float) ( controllerPortData[5] - 128 )/100;
        Left_Trigger_Val  = (float) ( controllerPortData[6] - 28 )/200;
        Right_Trigger_Val = (float) ( controllerPortData[7] - 28 )/200;
    }

    private static bool BitValue(byte b, int offset)
    {
        return (b & (1 << offset-1)) != 0;
    }

    public void Assign_Buttons_A_B_X_Y(byte [] controllerPortData)
    {
        byte data = controllerPortData[0];
        
        if (BitValue(data, 1))
        {
            Button_A = true;
        }
        if (BitValue(data, 2))
        {
            Button_B = true;
        }
        if (BitValue(data, 3))
        {
            Button_X = true;
        }
        if (BitValue(data, 4))
        {
            Button_Y = true;
        }
    }
    
    public void Assign_Buttons_DPAD(byte [] controllerPortData)
    {
        byte data = controllerPortData[0];
        
        if (BitValue(data, 5))
        {
            Button_LEFT = true;
        }
        if (BitValue(data, 6))
        {
            Button_RIGHT = true;
        }
        if (BitValue(data, 7))
        {
            Button_DOWN = true;
        }
        if (BitValue(data, 8))
        {
            Button_UP = true;
        }
    }
    
    public void Assign_Buttons_START_Z_R_L(byte [] controllerPortData)
    {
        byte data = controllerPortData[1];
        
        if (BitValue(data, 1))
        {
            Button_START = true;
        }
        if (BitValue(data, 2))
        {
            Button_Z = true;
        }
        if (BitValue(data, 3))
        {
            Button_R = true;
        }
        if (BitValue(data, 4))
        {
            Button_L = true;
        }
    }

}
