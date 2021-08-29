' *** SimpleBluetooth ***

' This example shows the usage of the watchdog for communication over Bluetooth, tested with the well known JY-MCU HC-05 and HC-06
'
' To help get you started, have a look at:
'    - http://www.instructables.com/id/Cheap-2-Way-Bluetooth-Connection-Between-Arduino-a/step4/Set-up-your-PC-for-serial-Bluetooth-communication/
'    - http://homepages.ihug.com.au/~npyner/Arduino/GUIDE_2BT.pdf
'    basically for JY-MCU HC-05 and HC-06 you only have to make sure that
'     1) the device is connected using a voltage divider
'     2) the serial speed set in your script is the same as the Bluetooth speed (by default 9600)
'     So, don't worry about discovery and pairing, CmdMessenger will do that for you.
'     
'      On Arduino side, use the same SimpleWatchdog.ino script as the previous example, but make sure the speed is set to 9600
'
' - Use bluetooth connection
' - Use auto scanning and connecting
' - Use watchdog 
Imports System
Imports System.Collections.Generic
Imports CommandMessenger
Imports CommandMessenger.Transport
Imports CommandMessenger.Transport.Bluetooth


Class SimpleWatchdog
    Private Enum CommandIds
        Identify
        ' Command to identify device
        TurnLedOn
        ' Command to request led to be turned on
    End Enum

    Public Property RunLoop As Boolean

    ' Most of the time you want to be sure you are connecting with the correct device.        
    Private Const CommunicationIdentifier As String = "BFAF4176-766E-436A-ADF2-96133C02B03C"

    ' You could also check for the first device that has the correct (sketch) application and version running
    'private const string CommunicationIdentifier = "SimpleWatchdog__1_0_1";

    Private Shared _transport As ITransport
    Private Shared _cmdMessenger As CmdMessenger
    Private Shared _connectionManager As ConnectionManager

    ' Setup function
    Public Sub Setup()
        ' Let's show all bluetooth devices
        ShowBluetoothInfo()

        ' Now let us set Bluetooth transport
        ' If you know your bluetooth device and you have already paired
        ' you can directly connect to you Bluetooth Device by adress adress.
        ' Under windows you can find the adresss at:
        '    Control Panel >> All Control Panel Items >> Devices and Printers
        '    Right-click on device >> properties >> Unique id
        _transport = New BluetoothTransport() With { _
            .CurrentBluetoothDeviceInfo = BluetoothUtils.DeviceByAdress("20:13:07:26:10:08")
            }

        ' Initialize the command messenger with the Serial Port transport layer
        ' Set if it is communicating with a 16- or 32-bit Arduino board
        ' Do not print newLine at end of command, to reduce data being sent
        _cmdMessenger = New CmdMessenger(_transport,BoardType.Bit16) With {
            .PrintLfCr = False
            }

        ' The Connection manager is capable or storing connection settings, in order to reconnect more quickly  
        ' the next time the application is run. You can determine yourself where and how to store the settings
        ' by supplying a class, that implements ISerialConnectionStorer. For convenience, CmdMessenger provides
        '  simple binary file storage functionality
        Dim bluetoothConnectionStorer = New BluetoothConnectionStorer("BluetoothConnectionManagerSettings.cfg")

        ' It is easier to let the BluetoothConnectionManager connection for you.
        ' It will:
        '  - Auto discover Bluetooth devices
        '  - If not yet paired, try to pair using the default Bluetooth passwords
        '  - See if the device responds with the correct CommunicationIdentifier
        ' Enable watchdog functionality.

        ' You can add PIN codes for specific devices

        ' You can also add PIN code to try on all unpaired devices
        ' (the following PINs are tried by default: 0000, 1111, 1234 )
        _connectionManager = New BluetoothConnectionManager(
            TryCast(_transport, BluetoothTransport),
            _cmdMessenger,
            CommandIds.Identify,
            CommunicationIdentifier,
            bluetoothConnectionStorer) With {
                .DevicePins = New Dictionary(Of String, String) From
                    {{"01:02:03:04:05:06", "6666"},
                    {"01:02:03:04:05:07", "7777"}},
                .GeneralPins = New List(Of String) From {"8888"}
                }

        ' Enable watchdog functionality.
        _connectionManager.WatchdogEnabled = True

        ' Show all connection progress on command line   
        AddHandler _connectionManager.Progress, Sub(sender, eventArgs)
                                                    ' If you want to reduce verbosity, you can only show events of level <=2 or ==1
                                                    If eventArgs.Level <= 3 Then
                                                        Console.WriteLine(eventArgs.Description)
                                                    End If
                                                End Sub

        ' If connection found, tell the arduino to turn the (internal) led on
        AddHandler _connectionManager.ConnectionFound, Sub(sender, eventArgs)
                                                           ' Create command
                                                           Dim sendCommand = New SendCommand(CommandIds.TurnLedOn)

                                                           ' Send command
                                                           _cmdMessenger.SendCommand(sendCommand)
                                                       End Sub

        ' Finally - activate connection manager
        _connectionManager.StartConnectionManager()
    End Sub

    ' Show Bluetooth information
    Private Shared Sub ShowBluetoothInfo()
        ' Show  adress of local primary bluetooth device 
        Console.WriteLine("Adress of the connected (primary) bluetooth device:")
        BluetoothUtils.PrintLocalAddress()
        Console.WriteLine("")

        'Show all paired bluetooth devices
        Console.WriteLine("All paired bluetooth devices:")
        BluetoothUtils.PrintPairedDevices()
        Console.WriteLine("")

        'Show all bluetooth devices, paired and unpaired. 
        ' Note that this takes a lot of time!
        Console.WriteLine("All Bluetooth devices, paired and unpaired:")
        BluetoothUtils.PrintAllDevices()
        Console.WriteLine("")

        ' Show Virtual serial ports associated with Bluetooth devices
        ' Note that CmdMessenger does not need these and will bypass them
        Console.WriteLine("Virtual serial ports associated with Bluetooth devices:")
        BluetoothUtils.PrintSerialPorts()
        Console.WriteLine("")
    End Sub

    ' Loop function
    Public Sub [Loop]()
        'Wait for key
        Console.ReadKey()
        ' Stop loop
        RunLoop = False
    End Sub

    ' Exit function
    Public Sub [Exit]()
        _connectionManager.Dispose()
        _cmdMessenger.Disconnect()
        _cmdMessenger.Dispose()
        _transport.Dispose()
    End Sub

End Class