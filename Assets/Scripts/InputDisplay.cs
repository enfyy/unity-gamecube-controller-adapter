using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
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

    public Dropdown PortDropdown;
    
    public Text DataText;
    public Text AnalogText;
    public Text CText;
    public Text RAnalog;
    public Text LAnalog;

    private float scaleFactor = 40;
    private Vector3 originalAnalogPos;
    private Vector3 originalCPos;
    private RectTransform analogTransform;
    private RectTransform cTransform;

    // Start is called before the first frame update
    void Start()
    {
        analogTransform = Analog.GetComponent<RectTransform>();
        cTransform = C.GetComponent<RectTransform>();
        
        originalAnalogPos = analogTransform.position;
        originalCPos = cTransform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Gcc.isReading)
            return;

        GccStatus input = Gcc.Input(PortDropdown.value);
        if (input == null)
            return;

        AnalogText.text   = "X: " + input.Left_xVal + " || Y: " + input.Left_yVal;
        CText.text   = "X: " + input.Right_xVal + " || Y: " + input.Right_yVal;
        
        LAnalog.text = input.Left_Trigger_Val.ToString();
        RAnalog.text = input.Right_Trigger_Val.ToString();
        string data = "[ ";
        foreach (var databyte in GccStatus.ControllerData)
        {
            data += databyte.ToString("X");
            data += " ";
        }

        data += "]";
        DataText.text = " Datastring: " + data;

        Vector2 newC = new Vector2(originalCPos.x + input.Right_xVal * scaleFactor, originalCPos.y + input.Right_yVal * scaleFactor); //TODO: move them correctly
        cTransform.position = newC;
        
        Vector2 newAnalog = new Vector2(originalAnalogPos.x + input.Left_xVal * scaleFactor, originalAnalogPos.y + input.Left_yVal * scaleFactor);
        analogTransform.position = newAnalog;
        
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
