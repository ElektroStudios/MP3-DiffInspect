Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text

Imports DevCase.Core.Application.Forms
Imports DevCase.Extensions
Imports DevCase.UI.Components

Imports NAudio.Wave

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

        Dim audioStreamDifferent As Boolean

        Dim channelsEqual As Boolean = Me.selectedMp3InfoObjectTop.Channels = Me.selectedMp3InfoObjectBottom.Channels
        Dim sampleRateEqual As Boolean = Me.selectedMp3InfoObjectTop.SampleRate = Me.selectedMp3InfoObjectBottom.SampleRate
        Dim bitsPerSampleEqual As Boolean = Me.selectedMp3InfoObjectTop.BitsPerSample = Me.selectedMp3InfoObjectBottom.BitsPerSample
        Dim bitrateEqual As Boolean = Me.selectedMp3InfoObjectTop.BitrateAverageKbps = Me.selectedMp3InfoObjectBottom.BitrateAverageKbps
        Dim durationEqual As Boolean = Me.selectedMp3InfoObjectTop.Duration = Me.selectedMp3InfoObjectBottom.Duration

        If Not channelsEqual OrElse
           Not sampleRateEqual OrElse
           Not bitsPerSampleEqual OrElse
           Not bitrateEqual OrElse
           Not durationEqual Then

            audioStreamDifferent = True
        End If

        If Not audioStreamDifferent Then
            Dim frameCountDifferent As Boolean =
                (Me.selectedMp3InfoObjectTop.FrameCount <> Me.selectedMp3InfoObjectBottom.FrameCount)

            If frameCountDifferent Then
                audioStreamDifferent = True

            Else ' Compare frame by frame.
                ' NOTE: This iteration is a LOT faster than using the Mp3FrameSequenceComparer class.

                Dim framesDifferent As Boolean

                Dim topFrames As ReadOnlyCollection(Of Mp3Frame) = Me.selectedMp3InfoObjectTop.Frames
                Dim bottomFrames As ReadOnlyCollection(Of Mp3Frame) = Me.selectedMp3InfoObjectBottom.Frames

                For i As Integer = 0 To topFrames.Count - 1
                    Dim topBytes As Byte() = topFrames(i).RawData
                    Dim bottomBytes As Byte() = bottomFrames(i).RawData

                    If topBytes.Length <> bottomBytes.Length Then
                        framesDifferent = True
                        Exit For
                    End If

                    For byteIndex As Integer = 0 To topBytes.Length - 1
                        If topBytes(byteIndex) <> bottomBytes(byteIndex) Then
                            framesDifferent = True
                            Exit For
                        End If
                    Next

                    If framesDifferent Then
                        Exit For
                    End If
                Next

                audioStreamDifferent = framesDifferent
            End If
        End If

        If audioStreamDifferent Then
            Me.LabelAudioStreamDiff.ForeColor = Color.IndianRed
            Me.LabelAudioStreamDiff.Text = "🟥 Audio stream different"
        Else
            Me.LabelAudioStreamDiff.ForeColor = Color.LimeGreen
            Me.LabelAudioStreamDiff.Text = "✅ Audio stream identical"
        End If

        Dim artistEqual As Boolean = Me.selectedMp3InfoObjectTop.Artist = Me.selectedMp3InfoObjectBottom.Artist
        Dim titleEqual As Boolean = Me.selectedMp3InfoObjectTop.Title = Me.selectedMp3InfoObjectBottom.Title
        Dim albumEqual As Boolean = Me.selectedMp3InfoObjectTop.Album = Me.selectedMp3InfoObjectBottom.Album
        Dim yearEqual As Boolean = Me.selectedMp3InfoObjectTop.Year = Me.selectedMp3InfoObjectBottom.Year

        Dim coverEqual As Boolean
        Using msCoverTop As New MemoryStream(), msCoverBottom As New MemoryStream()

            Me.selectedMp3InfoObjectTop.Cover?.Save(msCoverTop, Imaging.ImageFormat.Bmp)
            Me.selectedMp3InfoObjectBottom.Cover?.Save(msCoverBottom, Imaging.ImageFormat.Bmp)

            If msCoverTop.Length <> msCoverBottom.Length Then
                coverEqual = False
            Else
                msCoverTop.Position = 0
                msCoverBottom.Position = 0

                Dim bufferTop(UShort.MaxValue) As Byte
                Dim bufferBottom(UShort.MaxValue) As Byte
                Dim differenceFound As Boolean

                While Not differenceFound AndAlso (msCoverTop.Position < msCoverTop.Length)
                    Dim readTop As Integer = msCoverTop.Read(bufferTop, 0, bufferTop.Length)
                    Dim readBottom As Integer = msCoverBottom.Read(bufferBottom, 0, bufferBottom.Length)

                    For i As Integer = 0 To readTop - 1
                        If bufferTop(i) <> bufferBottom(i) Then
                            differenceFound = True
                            Exit For
                        End If
                    Next
                End While

                coverEqual = Not differenceFound
            End If
        End Using

        Dim sbOtherDiffs As New StringBuilder()

        If Not channelsEqual Then
            sbOtherDiffs.Append("Channels, ")
        End If

        If Not sampleRateEqual Then
            sbOtherDiffs.Append("Sample Rate, ")
        End If

        If Not bitsPerSampleEqual Then
            sbOtherDiffs.Append("Bits Per Sample, ")
        End If

        If Not bitrateEqual Then
            sbOtherDiffs.Append("Bitrate, ")
        End If

        If Not durationEqual Then
            sbOtherDiffs.Append("Duration, ")
        End If

        If Not coverEqual Then
            sbOtherDiffs.Append("Cover, ")
        End If

        If Not artistEqual OrElse
           Not titleEqual OrElse
           Not albumEqual OrElse
           Not yearEqual Then

            sbOtherDiffs.Append("Metadata tags, ")
        End If

        If sbOtherDiffs.Length = 0 Then
            Me.LabelOtherDifferencesResult.ForeColor = Color.LimeGreen
            Me.LabelOtherDifferencesResult.Text = "None"

        Else
            Dim value As String = sbOtherDiffs.ToString()
            value = value.TrimEnd({" "c, ","c})

            Me.LabelOtherDifferencesResult.ForeColor = Color.FromArgb(188, 188, 29)
            Me.LabelOtherDifferencesResult.Text = value
        End If

        ' Build the list of PropertyGrid labels that actually differ.
        ' These must match the DisplayName of each property exposed by the
        ' object bound to the PropertyGrid (check <DisplayName("...")> attributes
        ' or the raw property names if no attribute is set).
        Dim diffs As New List(Of String)()

        If Not artistEqual Then diffs.Add("Artist")
        If Not titleEqual Then diffs.Add("Title")
        If Not albumEqual Then diffs.Add("Album")
        If Not yearEqual Then diffs.Add("Year")
        If Not channelsEqual Then diffs.Add("Channels")
        If Not sampleRateEqual Then diffs.Add("Sample Rate")
        If Not bitsPerSampleEqual Then diffs.Add("Bits Per Sample")
        If Not bitrateEqual Then diffs.Add("Bitrate")
        If Not durationEqual Then diffs.Add("Duration")
        If Not coverEqual Then diffs.Add("Cover")

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
