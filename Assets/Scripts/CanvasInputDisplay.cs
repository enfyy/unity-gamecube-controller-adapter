using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInputDisplay : MonoBehaviour
{
    public GameObject A;
    public GameObject B;
    public GameObject X;
    public GameObject Y;
    
    public GameObject START;
    public GameObject Z;
    public GameObject R;
    public GameObject L;
    
    public GameObject LEFT;
    public GameObject RIGHT;
    public GameObject DOWN;
    public GameObject UP;

    public GameObject Analog;
    public GameObject C;

    public Text HexValuesTextfield;
    public Text LeftX;
    public Text RightX;
    public Text LeftY;
    public Text RightY;
    public Text RAnalog;
    public Text LAnalog;

    private float scaleFactor = 40;
    private Vector3 originalAnalogPos;
    private Vector3 originalCPos;

    // Start is called before the first frame update
    void Start()
    {
        originalAnalogPos = Analog.GetComponent<RectTransform>().position;
        originalCPos = C.GetComponent<RectTransform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Gcc.isReading)
            return;
        GccStatus input = Gcc.Input();

        LeftX.text   = input.Left_xVal.ToString();
        LeftY.text   = input.Left_yVal.ToString();
        RightX.text  = input.Right_xVal.ToString();
        RightY.text  = input.Right_yVal.ToString();
        LAnalog.text = input.Left_Trigger_Val.ToString();
        RAnalog.text = input.Right_Trigger_Val.ToString();
        HexValuesTextfield.text = "Button Data: " + input.ControllerData[2].ToString("X") + " | " + input.ControllerData[3].ToString("X");

        Vector2 new_C = new Vector2(originalCPos.x + input.Right_xVal * scaleFactor, originalCPos.y + input.Right_yVal * scaleFactor); //TODO: move them correctly
        C.GetComponent<RectTransform>().position = new_C;
        
        Vector2 new_Analog = new Vector2(originalAnalogPos.x + input.Left_xVal * scaleFactor, originalAnalogPos.y + input.Right_yVal * scaleFactor);
        Analog.GetComponent<RectTransform>().position = new_Analog;
        
        A.SetActive(input.Button_A);
        B.SetActive(input.Button_B);
        X.SetActive(input.Button_X);
        Y.SetActive(input.Button_Y);
        
        START.SetActive(input.Button_START);
        Z.SetActive(input.Button_Z);
        R.SetActive(input.Button_R);
        L.SetActive(input.Button_L);
        
        LEFT.SetActive(input.Button_LEFT);
        RIGHT.SetActive(input.Button_RIGHT);
        DOWN.SetActive(input.Button_DOWN);
        UP.SetActive(input.Button_UP);
    }
}
