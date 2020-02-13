using System;
using System.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using UnityEngine;

public class GamecubeAdapter
{
  private static readonly int VENDOR_ID = 0x057E;
  private static readonly int PRODUCT_ID = 0x0337;
  
  public static UsbDevice MyUsbDevice = null;
  public static UsbDeviceFinder UsbFinder = new UsbDeviceFinder(VENDOR_ID, PRODUCT_ID);
  public static UsbEndpointReader EndpointReader = null;
  public static ErrorCode ec;
  private static Thread InputThread;
  
  public static void Setup()
  {
    ec = ErrorCode.None;
    try
    {
      // Find and open the usb device.
      MyUsbDevice = UsbDevice.OpenUsbDevice(UsbFinder);
      if (MyUsbDevice == null) throw new Exception("Device Not Found.");
          
      EndpointReader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01, 4096, EndpointType.Interrupt);
      //MyUsbDevice.Configs[0].InterfaceInfoList[0].EndpointInfoList
    }
    catch (Exception ex)
    {
      Debug.LogError((ec != ErrorCode.None ? ec + ":" : String.Empty) + ex.Message);
    }
    
    InputThread = new Thread(ReadInputs);
    InputThread.Start();
  }

  public static void ReadInputs()
  {
    ec = ErrorCode.None;
    while (ec == ErrorCode.None)
    {
      byte[] readBuffer = new byte[37];
      int bytesRead;
      ec = EndpointReader.Read(readBuffer, 5000, out bytesRead);
      Debug.Log(bytesRead.ToString() + " bytes read");
      // Write that output to the console.
      Debug.Log(ec);
      Debug.Log(readBuffer.ToString());
    }
    InputThread.Abort();
    MyUsbDevice.Close();
  }
}