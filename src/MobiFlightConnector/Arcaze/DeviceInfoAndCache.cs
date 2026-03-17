using System;
using SimpleSolutions.Usb;

public class DeviceInfoAndCache
{
  public const Int32 ARCAZE_NUM_PORTS = 2;

  public DeviceInfo m_deviceInfo;
  public ArcazeHid  m_arcazeHid = new ArcazeHid();   // Contains the connection object
  public Int32[]    m_portValues;                      // Last known Port Content
  public Int32[]    m_portDirs;                        // Port Direction

  public DeviceInfoAndCache( DeviceInfo deviceInfo )
  {
    m_portValues = new Int32[ARCAZE_NUM_PORTS];
    m_portDirs   = new Int32[ARCAZE_NUM_PORTS];
    m_deviceInfo = deviceInfo;
  }

}
