

Friend Class Program
    Shared Sub Main()
        ' mimics Arduino calling structure
        Dim sendAndReceiveArguments = New SendAndReceiveArguments With {.RunLoop = True}
        sendAndReceiveArguments.Setup()
        Do While sendAndReceiveArguments.RunLoop
            sendAndReceiveArguments.Loop()
        Loop
        sendAndReceiveArguments.Exit()
    End Sub

End Class