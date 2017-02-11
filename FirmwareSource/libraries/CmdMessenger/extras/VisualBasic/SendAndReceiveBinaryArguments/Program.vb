

Friend Class Program
    Shared Sub Main()
        ' mimics Arduino calling structure
        Dim sendAndReceiveBinaryArguments = New SendAndReceiveBinaryArguments With {.RunLoop = True}
        sendAndReceiveBinaryArguments.Setup()
        Do While sendAndReceiveBinaryArguments.RunLoop
            sendAndReceiveBinaryArguments.Loop()
        Loop
        sendAndReceiveBinaryArguments.Exit()
    End Sub

End Class