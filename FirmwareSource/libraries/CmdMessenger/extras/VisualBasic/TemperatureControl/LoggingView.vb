Imports Microsoft.VisualBasic
Imports System.Drawing
Imports System.Windows.Forms

Namespace Tools
	Partial Public Class LoggingView
		Inherits ListBox
		Public Sub New()
			InitializeComponent()
						SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
			DrawMode = DrawMode.OwnerDrawFixed
			FollowLastItem = True
			MaxEntriesInListBox = 3000
		End Sub

		Private privateFollowLastItem As Boolean
		Public Property FollowLastItem() As Boolean
			Get
				Return privateFollowLastItem
			End Get
			Set(ByVal value As Boolean)
				privateFollowLastItem = value
			End Set
		End Property
		Private privateMaxEntriesInListBox As Integer
		Public Property MaxEntriesInListBox() As Integer
			Get
				Return privateMaxEntriesInListBox
			End Get
			Set(ByVal value As Integer)
				privateMaxEntriesInListBox = value
			End Set
		End Property


		Public Sub AddEntry(ByVal item As Object)
			BeginUpdate()
			Items.Add(item)

			If Items.Count > MaxEntriesInListBox Then
				Items.RemoveAt(0)
			End If

			If FollowLastItem Then
				TopIndex = Items.Count - 1
			End If
			EndUpdate()
		End Sub

		Protected Overrides Sub OnDrawItem(ByVal e As DrawItemEventArgs)
			If Items.Count > 0 Then
				e.DrawBackground()
				e.Graphics.DrawString(Items(e.Index).ToString(), e.Font, New SolidBrush(ForeColor), New PointF(e.Bounds.X, e.Bounds.Y))
			End If
			MyBase.OnDrawItem(e)
		End Sub
		Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
			Dim iRegion = New Region(e.ClipRectangle)
			e.Graphics.FillRegion(New SolidBrush(BackColor), iRegion)
			If Items.Count > 0 Then
				For i As Integer = 0 To Items.Count - 1
					Dim irect = GetItemRectangle(i)
					If e.ClipRectangle.IntersectsWith(irect) Then
						If (SelectionMode = SelectionMode.One AndAlso SelectedIndex = i) OrElse (SelectionMode = SelectionMode.MultiSimple AndAlso SelectedIndices.Contains(i)) OrElse (SelectionMode = SelectionMode.MultiExtended AndAlso SelectedIndices.Contains(i)) Then
							OnDrawItem(New DrawItemEventArgs(e.Graphics, Font, irect, i, DrawItemState.Selected, ForeColor, BackColor))
						Else
							OnDrawItem(New DrawItemEventArgs(e.Graphics, Font, irect, i, DrawItemState.Default, ForeColor, BackColor))
						End If
						iRegion.Complement(irect)
					End If
				Next i
			End If
			MyBase.OnPaint(e)
		End Sub
	End Class
End Namespace
