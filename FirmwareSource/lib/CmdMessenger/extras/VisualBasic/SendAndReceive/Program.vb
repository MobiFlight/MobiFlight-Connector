

Friend Class Program
    Shared Sub Main()

        ' mimics Arduino calling structure
        Dim sendAndReceive = New SendAndReceive With {.RunLoop = True}
        sendAndReceive.Setup()
        Do While sendAndReceive.RunLoop
            sendAndReceive.Loop()
        Loop
        sendAndReceive.Exit()

    End Sub
End Class