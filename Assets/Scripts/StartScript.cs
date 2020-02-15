using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MonoLibUsb;
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

    public void OnApplicationQuit()
    {
        if (Gcc.isReading)
        {
            Debug.Log("Force quit Adapter Polling...");
            Gcc.Stop();
        }
    }
}
