using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GccStatus
{
    public byte[] ControllerData;
    
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

    public GccStatus(byte[] controllerData)
    {
        ControllerData = controllerData;
        assignValuesFromData();
        Assign_Buttons_A_B_X_Y();
        Assign_Buttons_DPAD();
        Assign_Buttons_START_Z_R_L();
    }

    private void assignValuesFromData()
    {
        //read data and ste vals
        Left_xVal         = (float) ( ControllerData[4] - 128 )/100;
        Left_yVal         = (float) ( ControllerData[5] - 128 )/100;
        Right_xVal        = (float) ( ControllerData[6] - 128 )/100;
        Right_yVal        = (float) ( ControllerData[7] - 128 )/100;
        Left_Trigger_Val  = (float) ( ControllerData[8] - 128 )/100;
        Right_Trigger_Val = (float) ( ControllerData[9] - 128 )/100;
    }

    private bool bitValue(byte b, int offset)
    {
        return (b & (1 << offset-1)) != 0;
    }

    public void Assign_Buttons_A_B_X_Y()
    {
        byte data = ControllerData[2];
        
        if (bitValue(data, 1))
        {
            Button_A = true;
        }
        if (bitValue(data, 2))
        {
            Button_B = true;
        }
        if (bitValue(data, 3))
        {
            Button_X = true;
        }
        if (bitValue(data, 4))
        {
            Button_Y = true;
        }
    }
    
    public void Assign_Buttons_DPAD()
    {
        byte data = ControllerData[2];
        
        if (bitValue(data, 5))
        {
            Button_LEFT = true;
        }
        if (bitValue(data, 6))
        {
            Button_RIGHT = true;
        }
        if (bitValue(data, 7))
        {
            Button_DOWN = true;
        }
        if (bitValue(data, 8))
        {
            Button_UP = true;
        }
    }
    
    public void Assign_Buttons_START_Z_R_L()
    {
        byte data = ControllerData[3];
        
        if (bitValue(data, 1))
        {
            Button_START = true;
        }
        if (bitValue(data, 2))
        {
            Button_Z = true;
        }
        if (bitValue(data, 3))
        {
            Button_R = true;
        }
        if (bitValue(data, 4))
        {
            Button_L = true;
        }
    }

}
