Imports System.IO

Imports DevCase.Core.Application.Forms
Imports DevCase.Extensions
Imports DevCase.UI.Components

Public Class Form1

#Region " Private Fields "

    Private selectedMp3InfoObjectTop As Mp3Info

    Private selectedMp3InfoObjectBottom As Mp3Info

    Private _topHighlighter As PropertyGridHighlightOverlay
    Private _bottomHighlighter As PropertyGridHighlightOverlay

#End Region

#Region " Event Invocators "

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
    End Sub

#End Region

#Region " Event Handlers "

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Text = $"{My.Application.Info.Title} {My.Application.Info.Version.ToString(fieldCount:=3)} — by ElektroStudios"

        Me.TableLayoutPanel_FullTopArea.BorderStyle = BorderStyle.FixedSingle
        Me.TableLayoutPanel_FullBottomArea.BorderStyle = BorderStyle.FixedSingle

        FormExtensions.SetVisualTheme(Me, VisualTheme.VisualStudioDark, childControls:=True)

        Me.PictureBoxTop.AllowDrop = True
        Me.PictureBoxBottom.AllowDrop = True
    End Sub

    Private Sub Form_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Dim minWidthToMatchPictureBox As Integer = (Me.Width - Me.PropertyGridTop.Width) + Me.PictureBoxTop.Width
        Dim maxWidthTwoScreens As Integer = (Screen.FromControl(Me).Bounds.Width * 2)

        Me.MinimumSize = New Size(minWidthToMatchPictureBox, Me.Size.Height)
        Me.MaximumSize = New Size(maxWidthTwoScreens, Me.Size.Height)
    End Sub

    Private Sub Form_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        Me.ResetPropertyGridSplitterPositions()
    End Sub

    Private Sub ButtonLoadFileTop_Click(sender As Object, e As EventArgs) Handles ButtonLoadFileTop.Click

        If Me.OpenFileDialogTop.ShowDialog() = DialogResult.OK Then

            Dim filepath As String = Me.OpenFileDialogTop.FileName
            Me.LoadMP3FileOnTopArea(filepath)
        End If
    End Sub

    Private Sub ButtonLoadFileBottom_Click(sender As Object, e As EventArgs) Handles ButtonLoadFileBottom.Click

        If Me.OpenFileDialogBottom.ShowDialog() = DialogResult.OK Then

            Dim filepath As String = Me.OpenFileDialogBottom.FileName
            Me.LoadMP3FileOnBottomArea(filepath)
        End If
    End Sub

    Private Sub Top_And_Bottom_DragEnter(sender As Object, e As DragEventArgs) Handles _
        ButtonLoadFileTop.DragEnter,
        PictureBoxTop.DragEnter,
        PropertyGridTop.DragEnter,
        ButtonLoadFileBottom.DragEnter,
        PictureBoxBottom.DragEnter,
        PropertyGridBottom.DragEnter

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())

            e.Effect = If(files.Length = 1 AndAlso
                          String.Equals(Path.GetExtension(files(0)), ".mp3", StringComparison.OrdinalIgnoreCase),
                              DragDropEffects.Copy,
                              DragDropEffects.None)
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub Top_DragDrop(sender As Object, e As DragEventArgs) Handles _
        ButtonLoadFileTop.DragDrop,
        PictureBoxTop.DragDrop,
        PropertyGridTop.DragDrop

        Dim filepath As String = CType(e.Data.GetData(DataFormats.FileDrop), String()).Single()

        Me.LoadMP3FileOnTopArea(filepath)
    End Sub

    Private Sub Bottom_DragDrop(sender As Object, e As DragEventArgs) Handles _
        ButtonLoadFileBottom.DragDrop,
        PictureBoxBottom.DragDrop,
        PropertyGridBottom.DragDrop

        Dim filepath As String = CType(e.Data.GetData(DataFormats.FileDrop), String()).Single()

        Me.LoadMP3FileOnBottomArea(filepath)
    End Sub

    Private Sub PictureBoxes_Click(sender As Object, e As EventArgs) Handles _
        PictureBoxTop.Click,
        PictureBoxBottom.Click

        Dim pcb As PictureBox = DirectCast(sender, PictureBox)
        Dim img As Image = pcb.BackgroundImage
        If img Is Nothing Then
            Exit Sub
        End If

        Using floatPicture As New DevFloatingPicture(img)
            floatPicture.CloseOnEscapeKey = True
            floatPicture.CloseOnLeftMouseClick = True
            floatPicture.TopMost = True

            floatPicture.ShowDialog()
            Me.BringToFront()
        End Using

    End Sub

#End Region

#Region " Private Methods "

    Private Sub LoadMP3FileOnTopArea(filepath As String)

        Me.Cursor = Cursors.WaitCursor

        Me.ButtonLoadFileTop.Enabled = False
        Me.PropertyGridTop.Enabled = False
        Me.PictureBoxTop.Enabled = False

        Me.selectedMp3InfoObjectTop?.Dispose()

        Me.selectedMp3InfoObjectTop = New Mp3Info(filepath)
        Me.PropertyGridTop.SelectedObject = Me.selectedMp3InfoObjectTop
        Me.ResetPropertyGridSplitterPositions()

        Me.PictureBoxTop.BackgroundImage?.Dispose()
        Me.PictureBoxTop.BackgroundImage = Me.selectedMp3InfoObjectTop.Cover
        Me.PictureBoxTop.Cursor = If(Me.PictureBoxTop.BackgroundImage IsNot Nothing, Cursors.SizeAll, Cursors.Default)

        Me.DisplayDiffText()

        Me.ButtonLoadFileTop.Enabled = True
        Me.PropertyGridTop.Enabled = True
        Me.PictureBoxTop.Enabled = True

        If (Me.selectedMp3InfoObjectTop IsNot Nothing) AndAlso
           (Me.selectedMp3InfoObjectBottom IsNot Nothing) Then
            Me.LabelAudioStreamDiff.Visible = True
            Me.LabelOtherDifferences.Visible = True
            Me.LabelOtherDifferencesResult.Visible = True
        End If

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub LoadMP3FileOnBottomArea(filepath As String)

        Me.Cursor = Cursors.WaitCursor

        Me.ButtonLoadFileBottom.Enabled = False
        Me.PropertyGridBottom.Enabled = False
        Me.PictureBoxBottom.Enabled = False

        Me.selectedMp3InfoObjectBottom?.Dispose()

        Me.selectedMp3InfoObjectBottom = New Mp3Info(filepath)
        Me.PropertyGridBottom.SelectedObject = Me.selectedMp3InfoObjectBottom
        Me.ResetPropertyGridSplitterPositions()

        Me.PictureBoxBottom.BackgroundImage?.Dispose()
        Me.PictureBoxBottom.BackgroundImage = Me.selectedMp3InfoObjectBottom.Cover
        Me.PictureBoxBottom.Cursor = If(Me.PictureBoxBottom.BackgroundImage IsNot Nothing, Cursors.SizeAll, Cursors.Default)

        Me.DisplayDiffText()

        Me.ButtonLoadFileBottom.Enabled = True
        Me.PropertyGridBottom.Enabled = True
        Me.PictureBoxBottom.Enabled = True

        If (Me.selectedMp3InfoObjectTop IsNot Nothing) AndAlso
           (Me.selectedMp3InfoObjectBottom IsNot Nothing) Then
            Me.LabelAudioStreamDiff.Visible = True
            Me.LabelOtherDifferences.Visible = True
            Me.LabelOtherDifferencesResult.Visible = True
        End If

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub ResetPropertyGridSplitterPositions()

        If PropertyGridExtensions.IsInitialized(Me.PropertyGridTop) Then
            PropertyGridExtensions.SetSplitterPosition(Me.PropertyGridTop, 150)
        End If

        If PropertyGridExtensions.IsInitialized(Me.PropertyGridBottom) Then
            PropertyGridExtensions.SetSplitterPosition(Me.PropertyGridBottom, 150)
        End If
    End Sub

    Private Sub DisplayDiffText()
        If Me.selectedMp3InfoObjectTop Is Nothing OrElse Me.selectedMp3InfoObjectBottom Is Nothing Then
            Exit Sub
        End If

        Dim result As Mp3ComparisonResult = Mp3Comparer.Compare(Me.selectedMp3InfoObjectTop, Me.selectedMp3InfoObjectBottom)

        If result.AudioStreamIdentical Then
            Me.LabelAudioStreamDiff.ForeColor = Color.LimeGreen
            Me.LabelAudioStreamDiff.Text = "✅ Audio stream identical"
        Else
            Me.LabelAudioStreamDiff.ForeColor = Color.IndianRed
            Me.LabelAudioStreamDiff.Text = "🟥 Audio stream different"
        End If

        If result.HasOtherDifferences Then
            Me.LabelOtherDifferencesResult.ForeColor = Color.FromArgb(188, 188, 29)
            Me.LabelOtherDifferencesResult.Text = result.OtherDifferencesSummary
        Else
            Me.LabelOtherDifferencesResult.ForeColor = Color.LimeGreen
            Me.LabelOtherDifferencesResult.Text = "None"
        End If

        ' The list of PropertyGrid labels that actually differ.
        ' These must match the DisplayName of each property exposed by the
        ' object bound to the PropertyGrid (check <DisplayName("...")> attributes
        ' or the raw property names if no attribute is set).
        Dim diffs As New List(Of String)(result.DifferentPropertyDisplayNames)

        ' Clean up previous subclassers if they exist.
        If _topHighlighter IsNot Nothing Then _topHighlighter.Dispose()
        If _bottomHighlighter IsNot Nothing Then _bottomHighlighter.Dispose()

        Try
            _topHighlighter = New PropertyGridHighlightOverlay(Me.PropertyGridTop, diffs)
            _bottomHighlighter = New PropertyGridHighlightOverlay(Me.PropertyGridBottom, diffs)

            Me.PropertyGridTop.Refresh()
            Me.PropertyGridBottom.Refresh()

        Catch ex As Exception
            Throw New InvalidOperationException($"Failed to attach PropertyGrid overlays: {ex.Message}", ex)
        End Try
    End Sub

#End Region

End Class
