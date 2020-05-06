using System;
using UnityEngine;

public class GccStatus
{
    private static int centerInput = 128; // TODO: individual for X,Y for both sticks and calibrate-able ?

    private static readonly float InputNum = 80f; // number of possible inputs in a range (positive or negative)
    
    private static int highestVal_lx = 208; //TODO: configureable
    private static int lowestVal_lx = 48;
    private static int highestVal_ly = 208; 
    private static int lowestVal_ly = 48;
    private static int highestVal_rx = 208;
    private static int lowestVal_rx = 48;
    private static int highestVal_ry = 208; 
    private static int lowestVal_ry = 48;
    
    public int PortIndex;
    public static byte[] ControllerData;
    public bool IsActive;
    public static bool calibration_mode = false;
    
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

    private void CalibrateInputs(byte[] controllerPortData)
    {
        if (controllerPortData[2] > centerInput && controllerPortData[2] > highestVal_lx)
            highestVal_lx = controllerPortData[2];
        
        if (controllerPortData[2] < centerInput && controllerPortData[2] < lowestVal_lx)
            lowestVal_lx = controllerPortData[2];
        
        if (controllerPortData[3] > centerInput && controllerPortData[3] > highestVal_ly)
            highestVal_ly = controllerPortData[3];
        
        if (controllerPortData[3] < centerInput && controllerPortData[3] < lowestVal_ly)
            lowestVal_ly = controllerPortData[3];
        
        if (controllerPortData[4] > centerInput && controllerPortData[4] > highestVal_lx)
            highestVal_rx = controllerPortData[4];
        
        if (controllerPortData[4] < centerInput && controllerPortData[4] < lowestVal_lx)
            lowestVal_rx = controllerPortData[4];
        
        if (controllerPortData[5] > centerInput && controllerPortData[5] > highestVal_lx)
            highestVal_ry = controllerPortData[5];
        
        if (controllerPortData[5] < centerInput && controllerPortData[5] < lowestVal_lx)
            lowestVal_ry = controllerPortData[5];
    }

    private void AssignValuesFromData(byte[] controllerPortData)
    {
        if(calibration_mode)
            CalibrateInputs(controllerPortData);

        if (controllerPortData[2] >= centerInput)
            Left_xVal = Mathf.Clamp(Mathf.Floor((controllerPortData[2] - centerInput) / ( (highestVal_lx - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
        else
            Left_xVal = Mathf.Clamp(Mathf.Floor((controllerPortData[2] - centerInput) / ( Mathf.Abs(lowestVal_lx - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
       
        if (controllerPortData[3] >= centerInput)
            Left_yVal = Mathf.Clamp(Mathf.Floor((controllerPortData[3] - centerInput) / ( (highestVal_ly - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
        else
            Left_yVal = Mathf.Clamp(Mathf.Floor((controllerPortData[3] - centerInput) / ( Mathf.Abs(lowestVal_ly - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
        
        if (controllerPortData[4] >= centerInput)
            Right_xVal = Mathf.Clamp(Mathf.Floor((controllerPortData[4] - centerInput) / ( (highestVal_rx - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
        else
            Right_xVal = Mathf.Clamp(Mathf.Floor((controllerPortData[4] - centerInput) / ( Mathf.Abs(lowestVal_rx - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
       
        if (controllerPortData[5] >= centerInput)
            Right_yVal = Mathf.Clamp(Mathf.Floor((controllerPortData[5] - centerInput) / ( (highestVal_ry - centerInput) / InputNum)) * (1f / InputNum), -1, 1);
        else
            Right_yVal = Mathf.Clamp(Mathf.Floor((controllerPortData[5] - centerInput) / ( Mathf.Abs(lowestVal_ry - centerInput) / InputNum)) * (1f / InputNum), -1, 1);


        Left_Trigger_Val  = Mathf.Clamp(((float) (controllerPortData[6] - 28 )/200), 0, 1); //TODO: not sure bout this 'magical number' (28)
        
        Right_Trigger_Val = Mathf.Clamp(((float) (controllerPortData[7] - 28 )/200), 0, 1);
    }

    private static bool BitValue(byte b, int offset)
    {
        return (b & (1 << offset-1)) != 0;
    }

    public void Assign_Buttons_A_B_X_Y(byte [] controllerPortData)
    {
        byte data = controllerPortData[0];
        
        if (BitValue(data, 1))
            Button_A = true;
        
        if (BitValue(data, 2))
            Button_B = true;
        
        if (BitValue(data, 3))
            Button_X = true;
        
        if (BitValue(data, 4))
            Button_Y = true;
        
    }
    
    public void Assign_Buttons_DPAD(byte [] controllerPortData)
    {
        byte data = controllerPortData[0];
        
        if (BitValue(data, 5))
            Button_LEFT = true;
        
        if (BitValue(data, 6))
            Button_RIGHT = true;
        
        if (BitValue(data, 7))
            Button_DOWN = true;
        
        if (BitValue(data, 8))
            Button_UP = true;
        
    }
    
    public void Assign_Buttons_START_Z_R_L(byte [] controllerPortData)
    {
        byte data = controllerPortData[1];
        
        if (BitValue(data, 1))
            Button_START = true;
        
        if (BitValue(data, 2))
            Button_Z = true;
        
        if (BitValue(data, 3))
            Button_R = true;
        
        if (BitValue(data, 4))
            Button_L = true;
        
    }

}
