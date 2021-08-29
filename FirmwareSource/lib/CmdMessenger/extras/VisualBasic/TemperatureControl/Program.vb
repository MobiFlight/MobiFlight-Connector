Imports System
Imports System.Windows.Forms
Imports Microsoft.VisualBasic


Friend NotInheritable Class Program
    ''' <summary>
    ''' The main entry point for the application.
    ''' Note that the main code is not in this class
    ''' </summary>
    Private Sub New()
    End Sub
    <STAThread> _
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New ChartForm())
    End Sub
End Class