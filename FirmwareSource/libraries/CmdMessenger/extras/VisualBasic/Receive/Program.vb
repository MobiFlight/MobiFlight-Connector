

Friend Class Program
    Shared Sub Main()
        ' mimics Arduino calling structure
        Dim receive = New Receive With {.RunLoop = True}
        receive.Setup()
        Do While receive.RunLoop
            receive.Loop()
        Loop
        receive.Exit()
    End Sub

End Class