
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.ObjectModel

Imports MP3DiffInspect.Tests.Infrastructure

Imports NAudio.Wave

#End Region

#Region " Mp3InfoAudioPropertiesTests "

''' <summary>
''' Integration tests for the audio stream properties of <see cref="Mp3Info"/>,
''' using small MP3 files generated on the fly.
''' </summary>
<TestClass>
<TestCategory("Integration")>
Public NotInheritable Class Mp3InfoAudioPropertiesTests : Inherits Mp3FileTestBase

#Region " Stream Properties "

    ''' <summary>
    ''' Verifies that a stereo file reports two channels.
    ''' </summary>
    <TestMethod>
    Public Sub Channels_StereoFile_ReturnsTwo()

        Dim filePath As String = Me.CreateMp3File("stereo.mp3", New Mp3TestFileBuilder().WithMono(False))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual(2, info.Channels)
            Assert.AreEqual("2", info.ChannelsFormated)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that a mono file reports one channel.
    ''' </summary>
    <TestMethod>
    Public Sub Channels_MonoFile_ReturnsOne()

        Dim filePath As String = Me.CreateMp3File("mono.mp3", New Mp3TestFileBuilder().WithMono(True))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual(1, info.Channels)
            Assert.AreEqual("1", info.ChannelsFormated)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the sample rate is read from the frame headers.
    ''' </summary>
    '''
    ''' <param name="sampleRate">
    ''' The sample rate written to the file.
    ''' </param>
    <TestMethod>
    <DataRow(32000)>
    <DataRow(44100)>
    <DataRow(48000)>
    Public Sub SampleRate_ReturnsValueFromFrameHeader(sampleRate As Integer)

        Dim filePath As String = Me.CreateMp3File("samplerate.mp3", New Mp3TestFileBuilder().WithSampleRate(sampleRate))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual(sampleRate, info.SampleRate)
            Assert.AreEqual(sampleRate.ToString(), info.SampleRateFormated)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the average bitrate is read from the frame headers and formatted in kbps.
    ''' </summary>
    '''
    ''' <param name="kbps">
    ''' The bitrate written to the file.
    ''' </param>
    <TestMethod>
    <DataRow(128)>
    <DataRow(160)>
    <DataRow(192)>
    Public Sub BitrateAverageKbps_ReturnsValueFromFrameHeader(kbps As Integer)

        Dim filePath As String = Me.CreateMp3File("bitrate.mp3", New Mp3TestFileBuilder().WithBitrate(kbps))

        Using info As New Mp3Info(filePath)
            Assert.AreEqual(kbps, info.BitrateAverageKbps)
            Assert.AreEqual($"{kbps} kbps", info.BitrateAverageKbpsFormated)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the duration matches the number of frames and the sample rate.
    ''' </summary>
    '''
    ''' <param name="frameCount">
    ''' The number of frames written to the file.
    ''' </param>
    '''
    ''' <param name="sampleRate">
    ''' The sample rate written to the file.
    ''' </param>
    <TestMethod>
    <DataRow(40, 44100)>
    <DataRow(125, 48000)>
    <DataRow(1, 32000)>
    Public Sub Duration_MatchesFrameCountAndSampleRate(frameCount As Integer, sampleRate As Integer)

        Dim builder As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithFrameCount(frameCount).WithSampleRate(sampleRate)
        Dim filePath As String = Me.CreateMp3File("duration.mp3", builder)

        Using info As New Mp3Info(filePath)
            Mp3FileTestBase.AssertDurationsAreClose(builder.GetExpectedDuration(), info.Duration)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.DurationFormatted"/> uses the "hh:mm:ss.fff" format.
    ''' </summary>
    <TestMethod>
    Public Sub DurationFormatted_UsesExpectedFormat()

        ' 125 frames * 1152 samples / 48000 Hz = exactly 3 seconds.
        Dim builder As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithFrameCount(125).WithSampleRate(48000)
        Dim filePath As String = Me.CreateMp3File("formatted.mp3", builder)

        Using info As New Mp3Info(filePath)
            Assert.AreEqual("00:00:03.000", info.DurationFormatted)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that MP3 streams report zero bits per sample (it is a compressed format).
    ''' </summary>
    <TestMethod>
    Public Sub BitsPerSample_CompressedStream_ReturnsZero()

        Dim filePath As String = Me.CreateMp3File("bps.mp3", New Mp3TestFileBuilder())

        Using info As New Mp3Info(filePath)
            Assert.AreEqual(0, info.BitsPerSample)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the ID3 tags do not affect the audio stream properties.
    ''' </summary>
    <TestMethod>
    Public Sub StreamProperties_AreNotAffectedByTags()

        Dim png As Byte() = TestImageFactory.CreateSolidPng(16, 16, Color.Red)

        Dim untagged As String = Me.CreateMp3File("untagged.mp3", New Mp3TestFileBuilder())
        Dim tagged As String = Me.CreateMp3File("tagged.mp3", New Mp3TestFileBuilder().
                                                                  WithId3v2("Artist", "Title", "Album", "2001", png).
                                                                  WithId3v1("Artist", "Title", "Album", "2001"))

        Using a As New Mp3Info(untagged), b As New Mp3Info(tagged)
            Assert.AreEqual(a.Channels, b.Channels)
            Assert.AreEqual(a.SampleRate, b.SampleRate)
            Assert.AreEqual(a.BitrateAverageKbps, b.BitrateAverageKbps)
            Assert.AreEqual(a.Duration, b.Duration)
            Assert.AreEqual(a.FrameCount, b.FrameCount)
        End Using
    End Sub

#End Region

#Region " Frames "

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.Frames"/> and <see cref="Mp3Info.FrameCount"/> return every audio frame.
    ''' </summary>
    <TestMethod>
    Public Sub Frames_ReturnsAllAudioFrames()

        Dim filePath As String = Me.CreateMp3File("frames.mp3", New Mp3TestFileBuilder().
                                                                    WithFrameCount(37).
                                                                    WithId3v2(title:="Tagged").
                                                                    WithId3v1(title:="Tagged"))

        Using info As New Mp3Info(filePath)
            Assert.HasCount(37, info.Frames)
            Assert.AreEqual(37, info.FrameCount)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that the raw data of each frame matches the bytes written to the file.
    ''' </summary>
    <TestMethod>
    Public Sub Frames_RawDataMatchesWrittenBytes()

        Dim builder As Mp3TestFileBuilder = New Mp3TestFileBuilder().WithFrameCount(3).WithPayloadByte(&HAB)
        Dim filePath As String = Me.CreateMp3File("rawdata.mp3", builder)
        Dim expectedStream As Byte() = builder.BuildAudioStream()

        Using info As New Mp3Info(filePath)
            Dim actualStream As Byte() = info.Frames.SelectMany(Function(f As Mp3Frame) f.RawData).ToArray()
            CollectionAssert.AreEqual(expectedStream, actualStream)
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that <see cref="Mp3Info.Frames"/> is read once and cached.
    ''' </summary>
    <TestMethod>
    Public Sub Frames_IsCached()

        Dim filePath As String = Me.CreateMp3File("cached.mp3", New Mp3TestFileBuilder())

        Using info As New Mp3Info(filePath)
            Dim first As ReadOnlyCollection(Of Mp3Frame) = info.Frames
            Dim second As ReadOnlyCollection(Of Mp3Frame) = info.Frames

            Assert.AreSame(first, second)
        End Using
    End Sub

#End Region

#Region " File Handling "

    ''' <summary>
    ''' Verifies that reading the properties does not keep the file locked.
    ''' </summary>
    <TestMethod>
    Public Sub ReadingProperties_DoesNotLockTheFile()

        Dim filePath As String = Me.CreateMp3File("lock.mp3", New Mp3TestFileBuilder().WithId3v2("A", "T", "B", "2000"))

        Using info As New Mp3Info(filePath)
            Dim channels As Integer = info.Channels
            Dim artist As String = info.Artist
            Dim frameCount As Integer = info.FrameCount

            Assert.AreEqual(2, channels)
            Assert.AreEqual("A", artist)
            Assert.IsGreaterThan(0, frameCount)

            ' Must not throw IOException ("file is being used by another process").
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                Assert.IsGreaterThan(0L, fs.Length)
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that reading the audio properties of a non-existing file throws a <see cref="FileNotFoundException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub Channels_NonExistingFile_ThrowsFileNotFoundException()

        Using info As New Mp3Info(Me.GetTempFilePath("missing.mp3"))
            Assert.ThrowsExactly(Of FileNotFoundException)(Function() info.Channels)
        End Using
    End Sub

#End Region

End Class

#End Region
