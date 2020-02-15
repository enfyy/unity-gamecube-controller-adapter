using System;
using System.Runtime.InteropServices;
using System.Threading;
using LibUsbDotNet.Main;
using UnityEngine;
using MonoLibUsb;
using MonoLibUsb.Descriptors;
using MonoLibUsb.Profile;
using MonoLibUsb.Transfer;

public static class Gcc
{
  public static bool isReading = false; // turn off to stop the thread
  private static GccStatus[] ControllerPortData;
  
  private static MonoUsbTransferDelegate controlTransferDelegate;
  private static MonoUsbSessionHandle sessionHandle;
  private static MonoUsbDeviceHandle myDeviceHandle = null;
  private static byte endpoint_in;
  private static byte endpoint_out;
  
  private static byte[] pad_data = new byte[37];

  public static GccStatus Input(int index)
  {
    return ControllerPortData?[index];
  }

  // #################################################################
  // ## Shuts down the polling and releases interface.              ##
  // ## Run this before the application exits (OnApplicationQuit)   ##
  // #################################################################
  public static void Stop()
  {
    isReading = false;
    Shutdown();
  }

  private static bool IsWiiUAdapter(MonoUsbProfile profile)
  {
    if (profile.DeviceDescriptor.VendorID == 0x057e && profile.DeviceDescriptor.ProductID == 0x0337)
      return true;
    return false;
  }

  private static void ControlTransferCB(MonoUsbTransfer transfer)
  {
    ManualResetEvent completeEvent = GCHandle.FromIntPtr(transfer.PtrUserData).Target as ManualResetEvent;
    completeEvent.Set();
  }
  
  
  //############################################################################
  //## This needs to be called in order to start the Adapter reader thread.   ##
  //############################################################################
  public static void SetUp()
  {
    // Assign the control transfer delegate to the callback function. 
    controlTransferDelegate = ControlTransferCB;

    // Initialize the context.
    sessionHandle = new MonoUsbSessionHandle();
    if (sessionHandle.IsInvalid)
      throw new Exception(String.Format("Failed intializing libusb context.\n{0}:{1}",
        MonoUsbSessionHandle.LastErrorCode,
        MonoUsbSessionHandle.LastErrorString));

    MonoUsbProfileList profileList = new MonoUsbProfileList();

    try
    {
      // The list is initially empty.
      // Each time refresh is called the list contents are updated. 
      profileList.Refresh(sessionHandle);

      // Use the GetList() method to get a generic List of MonoUsbProfiles
      // Find the first profile that matches in MyVidPidPredicate.
      MonoUsbProfile myProfile = profileList.GetList().Find(IsWiiUAdapter);
      if (myProfile == null)
      {
        Debug.Log("Device not connected.");
        return;
      }
      else
      {
        Debug.Log("found the Device.");
      }

      // Open the device handle to perform I/O
      myDeviceHandle = myProfile.OpenDeviceHandle();
      if (myDeviceHandle.IsInvalid)
      {
        throw new Exception(String.Format("Failed opening device handle.\n{0}:{1}",
          MonoUsbDeviceHandle.LastErrorCode,
          MonoUsbDeviceHandle.LastErrorString));
      }
      else
      {
        Debug.Log("Device opened.");
      }

      //MonoUsbApi.ClaimInterface(myDeviceHandle, 0);
      MonoUsbConfigHandle configHandle;
      MonoUsbApi.GetConfigDescriptor(myProfile.ProfileHandle, 0, out configHandle);
      MonoUsbConfigDescriptor configDescriptor = new MonoUsbConfigDescriptor(configHandle);
      MonoUsbApi.ClaimInterface(myDeviceHandle, 0);

      foreach (MonoUsbInterface usbInterface in configDescriptor.InterfaceList)
      {
        foreach (var usbAltInterface in usbInterface.AltInterfaceList)
        {
          Debug.Log("Interface Num:" + usbAltInterface.bInterfaceNumber.ToString());

          foreach (var usbEndpoint in usbAltInterface.EndpointList)
          {
            if (usbEndpoint.bEndpointAddress.CompareTo((byte) UsbEndpointDirection.EndpointIn) > 0)
            {
              endpoint_in = usbEndpoint.bEndpointAddress;
              Debug.Log(endpoint_in.ToString());
            }
            else
            {
              endpoint_out = usbEndpoint.bEndpointAddress;
              Debug.Log(endpoint_out.ToString());
            }
          }
        }
      }
      byte[] payload = {0x13}; // value doesn't seem to matter ?
      int actualLength;
      var ret = MonoUsbApi.InterruptTransfer(myDeviceHandle, endpoint_out, payload, payload.Length, out actualLength, 1000);
      Debug.Log("initial payload read. Length: " + actualLength + " | Return code: " + ret );
      Debug.Log("Starting reader thread.");
      Thread adapterReadThread = new Thread(Read);
      adapterReadThread.Start();
    }
    catch(Exception ex)
    {
      Debug.Log(ex.Message);
    }
  }
  
  // ###########################################################################
  // ## This function runs in a separate Thread and polls the controller data ##
  // ##########################################################################
  private static void Read()
  {
    isReading = true;
    var ret = 0;
    while (ret == 0 && isReading)
    {
      int actualLength;
      ret = MonoUsbApi.InterruptTransfer(myDeviceHandle, endpoint_in, pad_data, pad_data.Length, out actualLength, 150);
      string result = "Data: ";
      for (var j = 0; j < pad_data.Length; j++)
      {
        result += pad_data[j].ToString("X") + " ";
      }
      ControllerPortData = GccStatus.ProcessControllerData(pad_data);
      //TODO: mutex ?
    }
    Debug.Log("Stopping reader thread.");
    Thread.CurrentThread.Abort();
    Shutdown();
  }

  private static void Shutdown()
  {
    MonoUsbApi.ReleaseInterface(myDeviceHandle, 0);
    myDeviceHandle.Close();
  }
}
