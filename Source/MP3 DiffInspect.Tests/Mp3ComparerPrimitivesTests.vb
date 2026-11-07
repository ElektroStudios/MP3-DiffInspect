
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports MP3DiffInspect.Tests.Infrastructure

Imports NAudio.Wave

#End Region

#Region " Mp3ComparerPrimitivesTests "

''' <summary>
''' Unit tests for the low-level comparison helpers of <see cref="Mp3Comparer"/>
''' (<see cref="Mp3Comparer.AreByteArraysEqual"/>, <see cref="Mp3Comparer.AreFrameSequencesEqual"/>
''' and <see cref="Mp3Comparer.AreImagesEqual"/>). No file is created on disk.
''' </summary>
<TestClass>
<TestCategory("Unit")>
Public NotInheritable Class Mp3ComparerPrimitivesTests

#Region " Helpers "

    ''' <summary>
    ''' Parses all the frames of an in-memory audio stream.
    ''' </summary>
    '''
    ''' <param name="builder">
    ''' The builder that produces the audio stream.
    ''' </param>
    '''
    ''' <returns>
    ''' The parsed frames.
    ''' </returns>
    Private Shared Function ReadFrames(builder As Mp3TestFileBuilder) As List(Of Mp3Frame)

        Dim frames As New List(Of Mp3Frame)
        Using ms As New MemoryStream(builder.BuildAudioStream())
            Dim frame As Mp3Frame = Mp3Frame.LoadFromStream(ms)
            While frame IsNot Nothing
                frames.Add(frame)
                frame = Mp3Frame.LoadFromStream(ms)
            End While
        End Using
        Return frames
    End Function

#End Region

#Region " AreByteArraysEqual "

    ''' <summary>
    ''' Verifies that two arrays with the same content are equal.
    ''' </summary>
    <TestMethod>
    Public Sub AreByteArraysEqual_SameContent_ReturnsTrue()

        Assert.IsTrue(Mp3Comparer.AreByteArraysEqual({1, 2, 3}, {1, 2, 3}))
    End Sub

    ''' <summary>
    ''' Verifies that two empty arrays are equal.
    ''' </summary>
    <TestMethod>
    Public Sub AreByteArraysEqual_BothEmpty_ReturnsTrue()

        Assert.IsTrue(Mp3Comparer.AreByteArraysEqual(Array.Empty(Of Byte)(), Array.Empty(Of Byte)()))
    End Sub

    ''' <summary>
    ''' Verifies that an array is equal to itself.
    ''' </summary>
    <TestMethod>
    Public Sub AreByteArraysEqual_SameInstance_ReturnsTrue()

        Dim data As Byte() = {9, 8, 7}
        Assert.IsTrue(Mp3Comparer.AreByteArraysEqual(data, data))
    End Sub

    ''' <summary>
    ''' Verifies that a single different byte, at any position, makes the arrays different.
    ''' </summary>
    '''
    ''' <param name="index">
    ''' The index of the different byte.
    ''' </param>
    <TestMethod>
    <DataRow(0)>
    <DataRow(2)>
    <DataRow(4)>
    Public Sub AreByteArraysEqual_OneByteDifferent_ReturnsFalse(index As Integer)

        Dim first As Byte() = {1, 2, 3, 4, 5}
        Dim second As Byte() = DirectCast(first.Clone(), Byte())
        second(index) = 255

        Assert.IsFalse(Mp3Comparer.AreByteArraysEqual(first, second))
    End Sub

    ''' <summary>
    ''' Verifies that arrays with different lengths are different, even if one is a prefix of the other.
    ''' </summary>
    <TestMethod>
    Public Sub AreByteArraysEqual_DifferentLength_ReturnsFalse()

        Assert.IsFalse(Mp3Comparer.AreByteArraysEqual({1, 2, 3}, {1, 2, 3, 4}))
        Assert.IsFalse(Mp3Comparer.AreByteArraysEqual({1, 2, 3, 4}, {1, 2, 3}))
    End Sub

    ''' <summary>
    ''' Verifies that a <see langword="Nothing"/> argument throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub AreByteArraysEqual_NullArgument_ThrowsArgumentNullException()

        Assert.ThrowsExactly(Of ArgumentNullException)(Function() Mp3Comparer.AreByteArraysEqual(Nothing, {1}))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() Mp3Comparer.AreByteArraysEqual({1}, Nothing))
    End Sub

#End Region

#Region " AreFrameSequencesEqual "

    ''' <summary>
    ''' Verifies that two frame sequences parsed from identical streams are equal.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_IdenticalStreams_ReturnsTrue()

        Dim first As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10).WithPayloadByte(1))
        Dim second As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10).WithPayloadByte(1))

        Assert.HasCount(10, first)
        Assert.IsTrue(Mp3Comparer.AreFrameSequencesEqual(first, second))
    End Sub

    ''' <summary>
    ''' Verifies that two empty frame sequences are equal.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_BothEmpty_ReturnsTrue()

        Assert.IsTrue(Mp3Comparer.AreFrameSequencesEqual(New List(Of Mp3Frame), New List(Of Mp3Frame)))
    End Sub

    ''' <summary>
    ''' Verifies that frame sequences with different payload data are different.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_DifferentPayload_ReturnsFalse()

        Dim first As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10).WithPayloadByte(1))
        Dim second As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10).WithPayloadByte(2))

        Assert.IsFalse(Mp3Comparer.AreFrameSequencesEqual(first, second))
    End Sub

    ''' <summary>
    ''' Verifies that a difference in only the last frame is detected.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_OnlyLastFrameDifferent_ReturnsFalse()

        Dim first As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10).WithPayloadByte(1))
        Dim second As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10).WithPayloadByte(1))
        Dim lastFrame As Mp3Frame = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(1).WithPayloadByte(2)).Single()
        second(second.Count - 1) = lastFrame

        Assert.IsFalse(Mp3Comparer.AreFrameSequencesEqual(first, second))
    End Sub

    ''' <summary>
    ''' Verifies that frames with different lengths (different bitrate) are different.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_DifferentFrameLength_ReturnsFalse()

        Dim first As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(5).WithBitrate(128))
        Dim second As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(5).WithBitrate(160))

        Assert.AreNotEqual(first(0).FrameLength, second(0).FrameLength)
        Assert.IsFalse(Mp3Comparer.AreFrameSequencesEqual(first, second))
    End Sub

    ''' <summary>
    ''' Verifies that frame sequences with a different number of frames are different.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_DifferentCount_ReturnsFalse()

        Dim first As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(10))
        Dim second As List(Of Mp3Frame) = Mp3ComparerPrimitivesTests.ReadFrames(New Mp3TestFileBuilder().WithFrameCount(11))

        Assert.IsFalse(Mp3Comparer.AreFrameSequencesEqual(first, second))
    End Sub

    ''' <summary>
    ''' Verifies that a <see langword="Nothing"/> argument throws <see cref="ArgumentNullException"/>.
    ''' </summary>
    <TestMethod>
    Public Sub AreFrameSequencesEqual_NullArgument_ThrowsArgumentNullException()

        Dim frames As New List(Of Mp3Frame)

        Assert.ThrowsExactly(Of ArgumentNullException)(Function() Mp3Comparer.AreFrameSequencesEqual(Nothing, frames))
        Assert.ThrowsExactly(Of ArgumentNullException)(Function() Mp3Comparer.AreFrameSequencesEqual(frames, Nothing))
    End Sub

#End Region

#Region " AreImagesEqual "

    ''' <summary>
    ''' Verifies that two missing images are considered equal.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_BothNothing_ReturnsTrue()

        Assert.IsTrue(Mp3Comparer.AreImagesEqual(Nothing, Nothing))
    End Sub

    ''' <summary>
    ''' Verifies that a missing image is different from an existing one, in both argument orders.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_OnlyOneNothing_ReturnsFalse()

        Using bmp As Bitmap = TestImageFactory.CreateSolidBitmap(4, 4, Color.Red)
            Assert.IsFalse(Mp3Comparer.AreImagesEqual(bmp, Nothing))
            Assert.IsFalse(Mp3Comparer.AreImagesEqual(Nothing, bmp))
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that two distinct images with the same size and pixels are equal.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_SamePixels_ReturnsTrue()

        Using first As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.Red),
              second As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.Red)

            Assert.IsTrue(Mp3Comparer.AreImagesEqual(first, second))
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that an image is equal to itself.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_SameInstance_ReturnsTrue()

        Using bmp As Bitmap = TestImageFactory.CreateSolidBitmap(8, 8, Color.Purple)
            Assert.IsTrue(Mp3Comparer.AreImagesEqual(bmp, bmp))
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that images with different colors are different.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_DifferentColor_ReturnsFalse()

        Using first As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.Red),
              second As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.Blue)

            Assert.IsFalse(Mp3Comparer.AreImagesEqual(first, second))
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that a single different pixel is detected.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_OnePixelDifferent_ReturnsFalse()

        Using first As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.White),
              second As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.White)

            second.SetPixel(15, 15, Color.Black)
            Assert.IsFalse(Mp3Comparer.AreImagesEqual(first, second))
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that images with different sizes are different.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_DifferentSize_ReturnsFalse()

        Using first As Bitmap = TestImageFactory.CreateSolidBitmap(16, 16, Color.Red),
              second As Bitmap = TestImageFactory.CreateSolidBitmap(16, 17, Color.Red)

            Assert.IsFalse(Mp3Comparer.AreImagesEqual(first, second))
        End Using
    End Sub

    ''' <summary>
    ''' Verifies that images larger than the internal comparison buffer (64 KB) are compared completely,
    ''' detecting a difference located beyond the first buffer block.
    ''' </summary>
    <TestMethod>
    Public Sub AreImagesEqual_LargeImages_DifferenceBeyondFirstBufferIsDetected()

        ' 256 x 256 x 4 bytes = 256 KB of pixel data (several buffer blocks).
        Using first As Bitmap = TestImageFactory.CreateSolidBitmap(256, 256, Color.Gray),
              second As Bitmap = TestImageFactory.CreateSolidBitmap(256, 256, Color.Gray)

            Assert.IsTrue(Mp3Comparer.AreImagesEqual(first, second))

            ' BMP rows are stored bottom-up, so the top-left pixel is near the end of the stream.
            second.SetPixel(0, 0, Color.Black)
            Assert.IsFalse(Mp3Comparer.AreImagesEqual(first, second))
        End Using
    End Sub

#End Region

End Class

#End Region
