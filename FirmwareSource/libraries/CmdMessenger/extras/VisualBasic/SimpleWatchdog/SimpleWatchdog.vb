' *** SimpleWatchdog ***

' This example shows the usage of the watchdog for communication over virtual serial port,
' 
' - Use bluetooth connection
' - Use auto scanning and connecting
' - Use watchdog 
Imports System
Imports CommandMessenger
Imports CommandMessenger.Transport
Imports CommandMessenger.Transport.Serial


Class SimpleWatchdog
    Private Enum CommandIds
        Identify    ' Command to identify device
        TurnLedOn   ' Command to request led to be turned on
    End Enum

    Public Property RunLoop As Boolean

    ' Most of the time you want to be sure you are connecting with the correct device.
    ' This can by done by checking for a specific Communication Identifier. 
    ' You can make a unique identifier per device, 
    ' see http://pragmateek.com/4-ways-to-generate-a-guid/         
    Private Const CommunicationIdentifier As String = "BFAF4176-766E-436A-ADF2-96133C02B03C"

    ' You could also check for the first device that has the correct (sketch) application and version running
    'private const string CommunicationIdentifier = "SimpleWatchdog__1_0_1";

    Private Shared _transport As ITransport
    Private Shared _cmdMessenger As CmdMessenger
    Private Shared _connectionManager As ConnectionManager

    ' Setup function
    Public Sub Setup()

        _transport = New SerialTransport With {
            .CurrentSerialSettings = New SerialSettings With {
                .DtrEnable = False
                }
            }
        ' some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.                        
        ' We do not need to set serial port and baud rate: it will be found by the connection manager                                                           

        ' Initialize the command messenger with the Serial Port transport layer
        ' Set if it is communicating with a 16- or 32-bit Arduino board
        ' Do not print newLine at end of command, to reduce data being sent
        _cmdMessenger = New CmdMessenger(_transport, BoardType.Bit16) With {
            .PrintLfCr = False}


        ' The Connection manager is capable or storing connection settings, in order to reconnect more quickly  
        ' the next time the application is run. You can determine yourself where and how to store the settings
        ' by supplying a class, that implements ISerialConnectionStorer. For convenience, CmdMessenger provides
        '  simple binary file storage functionality
        Dim serialConnectionStorer = New SerialConnectionStorer("SerialConnectionManagerSettings.cfg")

        ' We don't need to provide a handler for the Identify command - this is a job for Connection Manager.
        ' Enable watchdog functionality.

        ' Instead of scanning for the connected port, you can disable scanning and only try the port set in CurrentSerialSettings
        _connectionManager = New SerialConnectionManager(
            CType(_transport, SerialTransport),
            _cmdMessenger,
            CommandIds.Identify,
            CommunicationIdentifier,
            serialConnectionStorer) With {
                .WatchdogEnabled = True
                }

        ' Instead of scanning for the connected port, you can disable scanning and only try the port set in CurrentSerialSettings
        ' _connectionManager.DeviceScanEnabled = false

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


        'You can also do something when the connection is lost
        AddHandler _connectionManager.ConnectionTimeout, Sub(sender, eventArgs)
                                                             'Do something
                                                         End Sub


        ' Finally - activate connection manager
        _connectionManager.StartConnectionManager()
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