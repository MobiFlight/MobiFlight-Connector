

Friend Class Program
    Shared Sub Main()
        ' mimics Arduino calling structure
        Dim simpleWatchdog = New SimpleWatchdog With {.RunLoop = True}
        simpleWatchdog.Setup()
        Do While simpleWatchdog.RunLoop
            simpleWatchdog.Loop()
        Loop
        simpleWatchdog.Exit()
    End Sub

End Class