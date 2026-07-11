Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Imports DevCase.ThirdParty.TagLibSharp

Imports NAudio.Wave

Imports IPicture = TagLib.IPicture

Public NotInheritable Class Mp3Info : Implements IDisposable

    Private disposedValue As Boolean

    Private fileReaderPopulated As Boolean

    Private tagsPopulated As Boolean

    ' Removes track index prefix, for example "01. ", "01- " or "01 -"
    Private Shared ReadOnly trackIndexPrefixRgx As New Regex("^\d{1,3}(_|[\.-]|\.?\s*-|\s*-)\s*-?", RegexOptions.Compiled)

    <Browsable(False)>
    Public ReadOnly Property FileInfo As FileInfo

    <DisplayName("Directory")>
    <Browsable(True)>
    Public ReadOnly Property Directory As String
        Get
            Return Me.FileInfo.DirectoryName
        End Get
    End Property

    <DisplayName("File name")>
    <Browsable(True)>
    Public ReadOnly Property FileName As String
        Get
            Return Me.FileInfo.Name
        End Get
    End Property

    <DisplayName("File name (normalized)")>
    <Browsable(False)>
    Public ReadOnly Property FileNameNormalized As String
        Get
            If Me._fileNameNormalized Is Nothing Then
                Dim name As String = Path.GetFileNameWithoutExtension(Me.FileInfo.Name)

                Dim prefixCandidate As String = name.Substring(0, Math.Min(5, name.Length))
                If prefixCandidate?.Contains("."c) OrElse prefixCandidate?.Contains("-"c) Then
                    Dim restOfName As String = name.Substring(prefixCandidate.Length)
                    prefixCandidate = Mp3Info.trackIndexPrefixRgx.Replace(prefixCandidate, "")
                    name = prefixCandidate & restOfName
                End If

                Dim sb As New StringBuilder(capacity:=name.Length)
                For Each c As Char In name
                    If Char.IsLetterOrDigit(c) Then
                        sb.Append(c)
                    End If
                Next
                Me._fileNameNormalized = sb.ToString()
            End If

            Return Me._fileNameNormalized
        End Get
    End Property
    Private _fileNameNormalized As String = Nothing

    <DisplayName("Artist")>
    <Browsable(True)>
    Public ReadOnly Property Artist As String
        Get
            Me.PopulateTagProperties()
            Dim value As String = Me._artist
            Return If(Not String.IsNullOrEmpty(value), value, "N/A")
        End Get
    End Property
    Private _artist As String

    <DisplayName("Title")>
    <Browsable(True)>
    Public ReadOnly Property Title As String
        Get
            Me.PopulateTagProperties()
            Dim value As String = Me._title
            Return If(Not String.IsNullOrEmpty(value), value, "N/A")
        End Get
    End Property
    Private _title As String

    <DisplayName("Album")>
    <Browsable(True)>
    Public ReadOnly Property Album As String
        Get
            Me.PopulateTagProperties()
            Dim value As String = Me._album
            Return If(Not String.IsNullOrEmpty(value), value, "N/A")
        End Get
    End Property
    Private _album As String

    <Browsable(False)>
    Public ReadOnly Property Year As Integer
        Get
            Me.PopulateTagProperties()
            Return Me._year
        End Get
    End Property
    Private _year As Integer

    <DisplayName("Year")>
    <Browsable(True)>
    Public ReadOnly Property YearFormated As String
        Get
            Dim value As Integer = Me.Year
            Return If(value <> 0, value.ToString(), "N/A")
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property Channels As Integer
        Get
            Me.PopulateFileReaderProperties()
            Return Me._channels
        End Get
    End Property
    Private _channels As Integer

    <DisplayName("Channels")>
    <Browsable(True)>
    Public ReadOnly Property ChannelsFormated As String
        Get
            Dim value As Integer = Me.Channels
            Return If(value <> 0, value.ToString(), "N/A")
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property SampleRate As Integer
        Get
            Me.PopulateFileReaderProperties()
            Return Me._sampleRate
        End Get
    End Property
    Private _sampleRate As Integer

    <DisplayName("Sample Rate")>
    <Browsable(True)>
    Public ReadOnly Property SampleRateFormated As String
        Get
            Dim value As Integer = Me.SampleRate
            Return If(value <> 0, value.ToString(), "N/A")
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property BitsPerSample As Integer
        Get
            Me.PopulateFileReaderProperties()
            Return Me._bitsPerSample
        End Get
    End Property
    Private _bitsPerSample As Integer

    '<DisplayName("Bits Per Sample")>
    '<Browsable(True)>
    'Public ReadOnly Property BitsPerSampleFormated As String
    '    Get
    '        Dim value As Integer = Me.BitsPerSample
    '        Return If(value <> 0, value.ToString(), "N/A")
    '    End Get
    'End Property

    <Browsable(False)>
    Public ReadOnly Property BitrateAverageKbps As Integer
        Get
            Me.PopulateFileReaderProperties()
            Return Me._bitrateAverageKbps
        End Get
    End Property
    Private _bitrateAverageKbps As Integer

    <DisplayName("Bitrate")>
    <Browsable(True)>
    Public ReadOnly Property BitrateAverageKbpsFormated As String
        Get
            Dim value As Integer = Me.BitrateAverageKbps
            Return If(value <> 0, $"{value} kbps", "N/A")
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property Duration As TimeSpan
        Get
            Me.PopulateFileReaderProperties()
            Return Me._duration
        End Get
    End Property
    Private _duration As TimeSpan

    <DisplayName("Duration")>
    <Browsable(True)>
    Public ReadOnly Property DurationFormatted As String
        Get
            Dim value As TimeSpan = Me.Duration
            Return value.ToString("hh\:mm\:ss\.fff")
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property Cover As Image
        Get
            Me.PopulateTagProperties()
            Return Me._cover
        End Get
    End Property
    Private _cover As Image

    <Browsable(False)>
    Public ReadOnly Property Frames As ReadOnlyCollection(Of Mp3Frame)
        Get
            If Me._frames Is Nothing Then
                Using mp3Reader As New Mp3FileReader(Me.FileInfo.FullName)
                    Dim frameList As New List(Of Mp3Frame)
                    Dim frame As Mp3Frame = mp3Reader.ReadNextFrame()
                    While frame IsNot Nothing
                        frameList.Add(frame)
                        frame = mp3Reader.ReadNextFrame()
                    End While
                    Me._frames = frameList.AsReadOnly()
                End Using
            End If

            Return Me._frames
        End Get
    End Property
    Private _frames As ReadOnlyCollection(Of Mp3Frame)

    <DisplayName("Frame Count")>
    <Browsable(False)>
    Public ReadOnly Property FrameCount As Integer
        Get
            Return If(Me.Frames?.Count(), 0)
        End Get
    End Property

    Public Sub New(filepath As String)

        Me.New(New FileInfo(filepath))
    End Sub

    Public Sub New(mp3FileInfo As FileInfo)

        Me.FileInfo = mp3FileInfo
    End Sub

    Private Sub PopulateFileReaderProperties()

        If Me.fileReaderPopulated Then
            Exit Sub
        End If

        Using mp3Reader As New Mp3FileReader(Me.FileInfo.FullName)
            Me._channels = mp3Reader.Mp3WaveFormat.Channels
            Me._sampleRate = mp3Reader.Mp3WaveFormat.SampleRate
            Me._bitsPerSample = mp3Reader.Mp3WaveFormat.BitsPerSample
            Me._bitrateAverageKbps = CInt(Math.Round(mp3Reader.Mp3WaveFormat.AverageBytesPerSecond * 8 / 1000.0))
            Me._duration = mp3Reader.TotalTime
        End Using

        Me.fileReaderPopulated = True
    End Sub

    Private Sub PopulateTagProperties()

        If Me.tagsPopulated Then
            Exit Sub
        End If

        Using mp3File As New MP3File(Me.FileInfo)

            Dim id3v2 As DevCase.ThirdParty.TagLibSharp.ID3v2Tag = mp3File.Tags.ID3v2
            If Not id3v2.IsEmpty Then
                If String.IsNullOrWhiteSpace(Me._artist) Then
                    Me._artist = id3v2.Artist
                End If

                If String.IsNullOrWhiteSpace(Me._title) Then
                    Me._title = id3v2.Title
                End If

                If String.IsNullOrWhiteSpace(Me._album) Then
                    Me._album = id3v2.Album
                End If

                If Me._year = 0 Then
                    Me._year = id3v2.Year
                End If

                If Me._cover Is Nothing AndAlso id3v2.Pictures.Length > 0 Then
                    Dim pic As IPicture = id3v2.Pictures(0)
                    Using ms As New MemoryStream(pic.Data.Data)
                        Dim img As Image = Image.FromStream(ms)
                        Me._cover = New Bitmap(img)
                    End Using
                End If
            End If

            Dim id3v1 As DevCase.ThirdParty.TagLibSharp.ID3v1Tag = mp3File.Tags.ID3v1
            If Not id3v1.IsEmpty Then
                If String.IsNullOrWhiteSpace(Me._artist) Then
                    Me._artist = id3v1.Artist
                End If

                If String.IsNullOrWhiteSpace(Me._title) Then
                    Me._title = id3v1.Title
                End If

                If String.IsNullOrWhiteSpace(Me._album) Then
                    Me._album = id3v1.Album
                End If

                If Me._year = 0 Then
                    Me._year = id3v1.Year
                End If
            End If

        End Using

        Me.tagsPopulated = True
    End Sub

    Private Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Dispose managed objects
                Me._cover?.Dispose()
                Me._cover = Nothing

                ' Reset values
                Me._artist = Nothing
                Me._title = Nothing
                Me._album = Nothing
                Me._frames = Nothing
                Me._year = 0
                Me._channels = 0
                Me._sampleRate = 0
                Me._bitsPerSample = 0
                Me._bitrateAverageKbps = 0
                Me._duration = TimeSpan.Zero
            End If

            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

End Class