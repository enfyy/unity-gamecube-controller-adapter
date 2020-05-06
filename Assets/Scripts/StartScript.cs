using UnityEngine;

public class StartScript : MonoBehaviour
{
    public void StartPolling()
    {
        Debug.Log("Starting Adapter Polling...");
        Gcc.SetUp();
    }

    public void StopPolling()
    {
        Debug.Log("Stopping Adapter Polling...");
        Gcc.Stop();
    }
    
    public void CalibrationToggle()
    {
        Debug.Log("Toggled Calibration Mode");
        GccStatus.calibration_mode = !GccStatus.calibration_mode;
    }

    public void OnApplicationQuit()
    {
        if (Gcc.isReading)
        {
            Debug.Log("Force quit Adapter Polling...");
            Gcc.Stop();
        }
    }

   
}
